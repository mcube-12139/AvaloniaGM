using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using TypeScript;
using TypeScript.Field;
using TypeScript.Intermediate;
using TypeScript.Intermediate.FunctionBody;
using TypeScript.Intermediate.Instruction;
using TypeScript.Item;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.Type;
using UndertaleModLib;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleInstruction;

namespace AvaloniaGM.Services;

internal class CodeGenerator {
    readonly Dictionary<string, UndertaleCode> funCodes = [];
    UndertaleData? data;
    readonly Dictionary<(IType, IType), UndertaleVariable> methodsVariables = [];
    readonly Dictionary<string, int> stringIds = [];
    readonly Dictionary<string, UndertaleVariable> globalVariables = [];
    readonly Dictionary<string, UndertaleVariable> selfVariables = [];
    readonly Dictionary<IValueSymbol, UndertaleVariable> variables = [];
    readonly HashSet<ClassType> classTypes = [];
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

    internal void Generate(UndertaleData data, TypeScript.Intermediate.Program program, Dictionary<string, UndertaleCode> funCodes) {
        this.data = data;
        foreach ((string name, UndertaleCode code) in funCodes) {
            this.funCodes.Add(name, code);
        }

        foreach (Function fun in program.functions) {
            switch (fun.body) {
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
                            case ConstIntInstruction constInt: {
                                PushInt(constInt.value);
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
        classTypes.Clear();
        fieldIndices.Clear();
        userFunctions.Clear();
    }
}
