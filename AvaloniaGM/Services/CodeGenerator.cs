using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using TypeScript;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.Intermediate;
using TypeScript.Intermediate.FunctionBody;
using TypeScript.Intermediate.Instruction;
using TypeScript.Intermediate.Type;
using TypeScript.Item;
using TypeScript.Statement;
using TypeScript.Symbol;
using UndertaleModLib;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleInstruction;

namespace AvaloniaGM.Services;

internal class CodeGenerator {
    readonly Dictionary<string, UndertaleCode> funCodes = [];
    UndertaleData? data;
    readonly Dictionary<string, int> stringIds = [];
    readonly Dictionary<string, UndertaleVariable> globalVariables = [];
    readonly Dictionary<string, UndertaleVariable> selfVariables = [];
    readonly Dictionary<IValueSymbol, UndertaleVariable> variables = [];
    readonly Dictionary<StructField, short> fieldIndices = [];
    readonly Dictionary<Function, UserFunction> userFunctions = [];
    uint nextFunctionIndex = 0;

    uint byteCount;
    List<UndertaleInstruction> instructions = [];
    Stack<DataType> types = [];
    // 这个 1 是 arguments，固定存在。
    uint nextLocalIndex = 1;
    uint nextParameterIndex = 0;
    uint nextGlobalIndex = 0;
    UndertaleCodeLocals? codeLocals;
    readonly Dictionary<ILoopStatement, LoopContext> loopContexts = [];

    void Convert(DataType target) {
        DataType source = types.Pop();

        if (source != target) {
            instructions.Add(new() {
                Kind = Opcode.Conv,
                Type1 = source,
                Type2 = target
            });
            byteCount += 4;
        }

        types.Push(target);
    }

    void Call(UndertaleFunction fun, int parameterCount) {
        if (parameterCount == 1) {
            Convert(DataType.Variable);
        } else {
            throw new NotImplementedException();
        }

        for (int i = parameterCount; i != 0; --i) {
            types.Pop();
        }

        instructions.Add(new() {
            Kind = Opcode.Call,
            Type1 = DataType.Int32,
            ArgumentsCount = (ushort)parameterCount,
            ValueFunction = fun
        });
        byteCount += 8;

        types.Push(DataType.Variable);
    }

    void CallBuiltin(string name, int parameterCount) {
        Call(data!.Functions.EnsureDefined(name, data.Strings), parameterCount);
    }

    void PopUnused() {
        DataType type = types.Pop();
        instructions.Add(new() {
            Kind = Opcode.Popz,
            Type1 = type
        });
        byteCount += 4;
    }

    void PushShort(short value) {
        instructions.Add(new() {
            Kind = Opcode.PushI,
            Type1 = DataType.Int16,
            Type2 = DataType.Double,
            ValueShort = value
        });
        byteCount += 4;

        // int16 常量被按照 int32 类型处理
        types.Push(DataType.Int32);
    }

    void PushInt(int value) {
        if (value < 32768) {
            PushShort((short)value);
            return;
        }

        instructions.Add(new() {
            Kind = Opcode.Push,
            Type1 = DataType.Int32,
            Type2 = DataType.Double,
            ValueInt = value
        });
        byteCount += 8;

        types.Push(DataType.Int32);
    }

    (UndertaleString gameString, int id) GetString(string value) {
        UndertaleString gameString;

        if (!stringIds.TryGetValue(value, out int id)) {
            gameString = new(value);
            id = data!.Strings.Count;
            stringIds.Add(value, id);
            data.Strings.Add(gameString);
        } else {
            gameString = data!.Strings[id];
        }

        return (gameString, id);
    }

    internal void Generate(UndertaleData data, TypeScript.Intermediate.Program program, Dictionary<string, UndertaleCode> funCodes) {
        this.data = data;
        foreach ((string name, UndertaleCode code) in funCodes) {
            this.funCodes.Add(name, code);
        }

        foreach (Function fun in program.functions) {
            switch (fun.body) {
                case HostBody:
                    // 无事发生
                    break;
                case GuestBody guest: {
                    UndertaleCode? code = funCodes.GetValueOrDefault(fun.name);
                    if (code == null) {
                        // 自定义函数
                        string funName = $"{fun.name}_{nextFunctionIndex}";
                        ++nextFunctionIndex;

                        code = UndertaleCode.CreateEmptyEntry(data, $"gml_Script_{funName}");
                        UndertaleString nameStr = data!.Strings.MakeString(funName);
                        int scriptId = data.Scripts.Count;
                        UndertaleScript script = new() {
                            Name = nameStr,
                            Code = code
                        };

                        data.Scripts.Add(script);

                        UndertaleFunction underFun = data.Functions.EnsureDefined(funName, data.Strings);

                        UserFunction userFun = new(scriptId, script, underFun, code);
                        userFunctions.Add(fun, userFun);
                    }
                    codeLocals = data.CodeLocals.For(code);

                    foreach (IInstruction instruction in guest.instructions) {
                        switch (instruction) {
                            case CallInstruction call: {
                                if (call.fun!.body is HostBody host) {
                                    CallBuiltin(host.hostName, call.fun.parameters.Length);
                                } else {
                                    Call(userFunctions[call.fun].fun, call.fun.parameters.Length);
                                }
                                if (call.fun.resultType == TupleType.EMPTY) {
                                    // fuck Game Maker
                                    // 无法实现返回零大小类型的函数，函数只能返回 variable，所以对()特殊处理。
                                    PopUnused();
                                }
                                break;
                            }
                            case ConstIntInstruction constInt: {
                                PushInt(constInt.value);
                                break;
                            }
                            case ConstStringInstruction constString: {
                                (UndertaleString gameString, int id) = GetString(constString.value);

                                instructions.Add(new() {
                                    Kind = Opcode.Push,
                                    Type1 = DataType.String,
                                    ValueString = new UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>(gameString, id)
                                });
                                byteCount += 8;

                                types.Push(DataType.String);
                                break;
                            }
                            case PopInstruction: {
                                PopUnused();
                                break;
                            }
                        }
                    }

                    code.Replace(instructions);
                    code.Length = byteCount;
                    code.Offset = 0;
                    code.ArgumentsCount = 0;
                    code.LocalsCount = nextLocalIndex;
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        nextLocalIndex = 1;
        nextGlobalIndex = 0;
        nextFunctionIndex = 0;
        this.funCodes.Clear();
        this.data = null;
        stringIds.Clear();
        selfVariables.Clear();
        globalVariables.Clear();
        variables.Clear();
        fieldIndices.Clear();
        userFunctions.Clear();
    }
}
