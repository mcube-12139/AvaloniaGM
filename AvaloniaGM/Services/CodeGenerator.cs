using Avalonia.Controls.Documents;
using System;
using System.Collections.Generic;
using TypeScript.Intermediate;
using TypeScript.Intermediate.FunctionBody;
using TypeScript.Intermediate.Instruction;
using TypeScript.Intermediate.Type;
using UndertaleModLib;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleInstruction;

namespace AvaloniaGM.Services;

internal class CodeGenerator {
    readonly Dictionary<string, UndertaleCode> funCodes = [];
    UndertaleData? data;
    readonly Dictionary<string, int> stringIds = [];
    readonly Dictionary<Field, int> fieldIndices = [];
    readonly Dictionary<VirtualFunction, int> virtualFunctionIndices = [];
    readonly Dictionary<Implementation, UndertaleVariable> metaObjectVariables = [];
    readonly Dictionary<string, UndertaleVariable> selfVariables = [];
    readonly Dictionary<Parameter, UndertaleVariable> parameterVariables = [];
    readonly List<(bool used, UndertaleVariable variable)> hiddenLocalVariables = [];
    // Program 定义的局域量，隐藏量不算在内。
    readonly List<UndertaleVariable> localVariables = [];
    readonly Dictionary<Function, UserFunction> userFunctions = [];
    uint nextFunctionIndex = 0;

    uint byteCount;
    readonly List<UndertaleInstruction> instructions = [];
    readonly List<uint> instructionOffsets = [];
    readonly Dictionary<int, List<(uint offset, UndertaleInstruction instruction)>> instructionOffsetPromises = [];
    Stack<DataType> types = [];
    // 这个 1 是 arguments，固定存在。
    uint nextLocalIndex = 1;
    uint nextParameterIndex = 0;
    uint nextGlobalIndex = 0;
    UndertaleCodeLocals? codeLocals;

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
            // fuck Game Maker
            // 中间层实参正序入栈，而目标机实参倒序入栈，为了不用把所有 API 的参数倒过来写，导致被用户打死，使用隐藏量。
            UseHiddenVariables(parameterCount, variables => {
                foreach (UndertaleVariable variable in variables) {
                    Store(variable, VariableType.Normal);
                }

                foreach (UndertaleVariable variable in variables) {
                    Load(variable, VariableType.Normal);
                }
            });
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

    void Store(UndertaleVariable variable, VariableType variableType) {
        if (variableType == VariableType.Array) {
            types.Pop();
            types.Pop();
        }

        DataType valueType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Pop,
            Type1 = DataType.Variable,
            Type2 = valueType,
            ValueVariable = variable,
            TypeInst = variableType == VariableType.Normal ? variable.InstanceType : InstanceType.Undefined,
            ReferenceType = variableType
        });
        byteCount += 8;
    }

    void Load(UndertaleVariable variable, VariableType variableType) {
        if (variableType == VariableType.Array) {
            types.Pop();
            types.Pop();
        }

        Opcode opcode;

        if (variableType == VariableType.Normal) {
            // Normal 量的操作码可能不同
            if (variable.InstanceType == InstanceType.Local) {
                opcode = Opcode.PushLoc;
            } else if (variable.InstanceType == InstanceType.Self) {
                if (variable.VarID == (int)InstanceType.Builtin) {
                    opcode = Opcode.PushBltn;
                } else {
                    opcode = Opcode.Push;
                }
            } else if (variable.InstanceType == InstanceType.Global) {
                opcode = Opcode.PushGlb;
            } else {
                throw new NotImplementedException();
            }
        } else {
            // Array 量的操作码必须是 Push
            opcode = Opcode.Push;
        }

        instructions.Add(new() {
            Kind = opcode,
            Type1 = DataType.Variable,
            ValueVariable = variable,
            ReferenceType = variableType,
            TypeInst = variableType == VariableType.Normal ? variable.InstanceType : InstanceType.Undefined
        });
        byteCount += 8;

        types.Push(DataType.Variable);
    }

    void Multiply() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Mul,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            types.Push(DataType.Double);
        } else {
            types.Push(DataType.Variable);
        }
    }

    void Divide() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Div,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            types.Push(DataType.Double);
        } else {
            types.Push(DataType.Variable);
        }
    }

    void Modulo() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Mod,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            types.Push(DataType.Double);
        } else {
            types.Push(DataType.Variable);
        }
    }

    void Add() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Add,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else if (leftType == DataType.String && rightType == DataType.String) {
            types.Push(DataType.String);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            types.Push(DataType.Double);
        } else {
            types.Push(DataType.Variable);
        }
    }

    void Subtract() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Sub,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            types.Push(DataType.Double);
        } else {
            types.Push(DataType.Variable);
        }
    }

    void LeftShift() {
    }

    void RightShift() {
    }

    /// <summary>
    /// <para>将非(int32, int32)且非(int64, int64)类型值转换成(int64, int64)，用于执行位运算。</para>
    /// <para>可能创建隐藏量。</para>
    /// </summary>
    /// <param name="leftType"></param>
    /// <param name="rightType"></param>
    /// <returns></returns>
    (DataType, DataType, DataType) BitOperationConvert(DataType leftType, DataType rightType) {
        (DataType, DataType, DataType) result;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            result = (DataType.Int32, DataType.Int32, DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            result = (DataType.Int64, DataType.Int64, DataType.Int64);
        } else {
            types.Push(leftType);
            types.Push(rightType);

            UseHiddenVariables(1, variables => {
                Store(variables[0], VariableType.Normal);

                Convert(DataType.Int64);
                Load(variables[0], VariableType.Normal);
                Convert(DataType.Int64);
            });

            types.Pop();
            types.Pop();

            result = (DataType.Int64, DataType.Int64, DataType.Int64);
        }

        return result;
    }

    void BitAnd() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();
        (leftType, rightType, DataType resultType) = BitOperationConvert(leftType, rightType);

        instructions.Add(new() {
            Kind = Opcode.And,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        types.Push(resultType);
    }

    void BitXor() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();
        (leftType, rightType, DataType resultType) = BitOperationConvert(leftType, rightType);

        instructions.Add(new() {
            Kind = Opcode.Xor,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        types.Push(resultType);
    }

    void BitOr() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();
        (leftType, rightType, DataType resultType) = BitOperationConvert(leftType, rightType);

        instructions.Add(new() {
            Kind = Opcode.Or,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        types.Push(resultType);
    }

    void Compare(ComparisonType comparisonType) {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Cmp,
            ComparisonKind = comparisonType,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        types.Push(DataType.Boolean);
    }

    UndertaleInstruction Branch() {
        UndertaleInstruction result = new() {
            Kind = Opcode.B
        };

        instructions.Add(result);
        byteCount += 4;

        return result;
    }

    UndertaleInstruction BranchTrue() {
        UndertaleInstruction result = new() {
            Kind = Opcode.Bt
        };

        instructions.Add(result);
        byteCount += 4;

        types.Pop();

        return result;
    }

    UndertaleInstruction BranchFalse() {
        UndertaleInstruction result = new() {
            Kind = Opcode.Bf
        };

        instructions.Add(result);
        byteCount += 4;

        types.Pop();

        return result;
    }

    void Return() {
        Convert(DataType.Variable);
        types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Ret,
            Type1 = DataType.Variable
        });
        byteCount += 4;
    }

    void Exit() {
        instructions.Add(new() {
            Kind = Opcode.Exit,
            Type1 = DataType.Int32
        });
        byteCount += 4;
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

    UndertaleVariable AddGlobalVariable(string prefix, bool builtin) {
        string name = $"{prefix}_{nextGlobalIndex}";
        ++nextGlobalIndex;
        (UndertaleString gameString, int id) = GetString(name);

        return data!.Variables.Define(gameString, id, InstanceType.Global, builtin, data);
    }

    UndertaleVariable AddLocalVariable() {
        uint variableId = nextLocalIndex;
        ++nextLocalIndex;

        (UndertaleString gameString, int id) = GetString($"loc_{variableId}");

        UndertaleVariable variable = data!.Variables.DefineLocal(data, (int)variableId, gameString, id);
        codeLocals!.Locals.Add(new() {
            Index = (uint)id,
            Name = gameString,
        });

        if (nextLocalIndex > data.MaxLocalVarCount) {
            data.MaxLocalVarCount = nextLocalIndex;
        }

        return variable;
    }

    UserFunction GetUserFunction(Function fun) {
        if (!userFunctions.TryGetValue(fun, out UserFunction? result)) {
            string funName = $"{fun.name}_{nextFunctionIndex}";
            ++nextFunctionIndex;

            UndertaleCode code = UndertaleCode.CreateEmptyEntry(data, $"gml_Script_{funName}");
            UndertaleString nameStr = data!.Strings.MakeString(funName);
            int scriptId = data.Scripts.Count;
            UndertaleScript script = new() {
                Name = nameStr,
                Code = code
            };

            data.Scripts.Add(script);

            UndertaleFunction underFun = data.Functions.EnsureDefined(funName, data.Strings);

            result = new(scriptId, script, underFun, code);
            userFunctions.Add(fun, result);
        }

        return result;
    }

    void UseHiddenVariables(int count, Action<UndertaleVariable[]> useFun) {
        var variables = new UndertaleVariable[count];
        Span<int> indices = stackalloc int[count];
        int foundCount = 0;
        // 查找当前未使用的隐藏量
        for (int i = 0; i != hiddenLocalVariables.Count; ++i) {
            (bool used, UndertaleVariable variable) = hiddenLocalVariables[i];
            if (!used) {
                hiddenLocalVariables[i] = (true, variable);
                variables[foundCount] = variable;
                indices[foundCount] = i;
                ++foundCount;

                if (foundCount == count) {
                    break;
                }
            }
        }

        if (foundCount < count) {
            // 分配不够的隐藏量
            int leftCount = count - foundCount;
            for (int i = 0; i != leftCount; ++i) {
                UndertaleVariable variable = AddLocalVariable();
                variables[foundCount] = variable;
                indices[foundCount] = hiddenLocalVariables.Count;
                ++foundCount;

                hiddenLocalVariables.Add((true, variable));
            }
        }

        useFun(variables);
        // 释放
        for (int i = 0; i != count; ++i) {
            int index = indices[i];
            hiddenLocalVariables[index] = (false, variables[i]);
        }
    }

    UndertaleVariable GetSelfVariable(string name, bool builtin) {
        (UndertaleString gameString, int id) = GetString(name);

        if (!selfVariables.TryGetValue(name, out UndertaleVariable? result)) {
            result = data!.Variables.Define(gameString, id, InstanceType.Self, builtin, data);
            selfVariables.Add(name, result);
        }

        return result;
    }

    void SetBranchPromise(UndertaleInstruction instruction, int index, uint start) {
        if (index < instructionOffsets.Count) {
            instruction.JumpOffset = (int)(instructionOffsets[index] - start) / 4;
        } else {
            if (!instructionOffsetPromises.TryGetValue(index, out var promises)) {
                instructionOffsetPromises.Add(index, [(start, instruction)]);
            } else {
                promises.Add((start, instruction));
            }
        }
    }

    int GetFieldIndex(Field field) => fieldIndices[field];

    bool IsZeroSized(IType type) {
        if (type is TupleType tupleType) {
            foreach (IType elementType in tupleType.elementTypes) {
                if (!IsZeroSized(elementType)) {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    internal void Generate(UndertaleData data, TypeScript.Intermediate.Program program, Dictionary<string, UndertaleCode> funCodes) {
        this.data = data;
        foreach ((string name, UndertaleCode code) in funCodes) {
            this.funCodes.Add(name, code);
        }

        foreach (ClassType c in program.classes) {
            int index = 0;
            foreach (Field field in c.fields) {
                fieldIndices[field] = index;
                ++index;
            }
        }

        foreach (InterfaceType i in program.interfaces) {
            int index = 1;
            foreach (VirtualFunction fun in i.functions) {
                virtualFunctionIndices[fun] = index;
                ++index;
            }
        }

        foreach (Function fun in program.functions) {
            switch (fun.body) {
                case HostBody:
                    // 无事发生
                    break;
                case GuestBody guest: {
                    UndertaleCode? gmCode = funCodes.GetValueOrDefault(fun.name);
                    // 自定义函数
                    UndertaleCode code = gmCode ?? GetUserFunction(fun).code;
                    codeLocals = data.CodeLocals.For(code);

                    if (gmCode != null) {
                        // todo: 确定静态期代码位置
                        // 生成元对象
                        foreach (Implementation impl in program.implementations.Values) {
                            UndertaleVariable variable = AddGlobalVariable("meta", false);
                            metaObjectVariables[impl] = variable;
                            foreach ((VirtualFunction virtualFun, Function implFun) in impl.functions) {
                                int index = virtualFunctionIndices[virtualFun];
                                UserFunction userFun = GetUserFunction(implFun);

                                PushInt(userFun.scriptId);
                                PushShort((short)InstanceType.Global);
                                PushInt(index);
                                Store(variable, VariableType.Array);
                            }
                        }
                    }

                    for (int i = 0; i != fun.parameters.Length; ++i) {
                        parameterVariables.Add(fun.parameters[i], GetSelfVariable($"argument{i}", true));
                    }

                    for (int i = 0; i != guest.localTypes.Length; ++i) {
                        localVariables.Add(AddLocalVariable());
                    }

                    for (int i = 0; i != guest.instructions.Length; ++i) {
                        instructionOffsets.Add(byteCount);

                        IInstruction instruction = guest.instructions[i];

                        switch (instruction) {
                            case CallInstruction call: {
                                if (call.fun!.body is HostBody host) {
                                    CallBuiltin(host.hostName, call.fun.parameters.Length);
                                } else {
                                    Call(userFunctions[call.fun].fun, call.fun.parameters.Length);
                                }
                                if (IsZeroSized(call.fun.resultType)) {
                                    // fuck Game Maker
                                    // 函数只能返回 variable，所以对零大小类型特殊处理。
                                    PopUnused();
                                }
                                break;
                            }
                            case CallPointerInstruction callPointer: {
                                CallBuiltin("script_execute", callPointer.type.parameterTypes.Length + 1);
                                if (IsZeroSized(callPointer.type.resultType)) {
                                    PopUnused();
                                }
                                break;
                            }
                            case CallVirtualInstruction callVirtual: {
                                // 0: 元对象
                                // 1: 接口对象
                                // 2..: 参数
                                int parameterCount = callVirtual.fun!.parameters.Length + 1;
                                UseHiddenVariables(parameterCount, variables => {
                                    for (int variableIndex = parameterCount - 1; variableIndex != 0; --variableIndex) {
                                        Store(variables[variableIndex], VariableType.Normal);
                                    }

                                    for (int variableIndex = parameterCount - 1; variableIndex != 1; --variableIndex) {
                                        Load(variables[variableIndex], VariableType.Normal);
                                    }

                                    // 载入类对象
                                    PushShort((short)InstanceType.Local);
                                    PushShort(0);
                                    Load(variables[1], VariableType.Array);

                                    // 载入元对象
                                    PushShort((short)InstanceType.Local);
                                    PushShort(1);
                                    Load(variables[1], VariableType.Array);
                                    Store(variables[0], VariableType.Normal);

                                    // 载入 script id
                                    PushShort((short)InstanceType.Local);
                                    PushInt(virtualFunctionIndices[callVirtual.fun]);
                                    Load(variables[0], VariableType.Array);
                                });

                                for (int parameterIndex = 0; parameterIndex != parameterCount; ++parameterIndex) {
                                    types.Pop();
                                }

                                instructions.Add(new() {
                                    Kind = Opcode.Call,
                                    Type1 = DataType.Int32,
                                    ArgumentsCount = (ushort)parameterCount,
                                    ValueFunction = data!.Functions.EnsureDefined("script_execute", data.Strings)
                                });
                                byteCount += 8;

                                types.Push(DataType.Variable);

                                if (IsZeroSized(callVirtual.fun.resultType)) {
                                    PopUnused();
                                }
                                break;
                            }
                            case StoreLocalInstruction storeLocal: {
                                Store(localVariables[storeLocal.index], VariableType.Normal);
                                break;
                            }
                            case LoadLocalInstruction loadLocal: {
                                Load(localVariables[loadLocal.index], VariableType.Normal);
                                break;
                            }
                            case StoreArgumentInstruction storeArgument: {
                                Store(parameterVariables[storeArgument.parameter], VariableType.Normal);
                                break;
                            }
                            case LoadArgumentInstruction loadArgument: {
                                Load(parameterVariables[loadArgument.parameter], VariableType.Normal);
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
                            case ConstFunctionInstruction constFun: {
                                if (!userFunctions.TryGetValue(constFun.fun!, out UserFunction? userFun)) {
                                    throw new Exception("不能求值 host 函数");
                                }

                                PushInt(userFun.scriptId);
                                break;
                            }
                            case NewArrayInstruction newArray: {
                                CallBuiltin("@@NewGMLArray@@", newArray.length);
                                break;
                            }
                            case NewClassInstruction newClass: {
                                UseHiddenVariables(1, variables => {
                                    UndertaleVariable variable = variables[0];

                                    PushInt(newClass.fields.Length);
                                    CallBuiltin("array_create", 1);
                                    if (newClass.fields.Length != 0) {
                                        Store(variable, VariableType.Normal);

                                        for (int i = newClass.fields.Length - 1; i != -1; --i) {
                                            PushShort((short)InstanceType.Local);
                                            PushInt(GetFieldIndex(newClass.fields[i]));
                                            Store(variable, VariableType.Array);
                                        }

                                        Load(variable, VariableType.Normal);
                                    }
                                });
                                break;
                            }
                            case UpcastInstruction upcast: {
                                if (
                                    upcast.source is ClassType classType
                                    && upcast.target is InterfaceType interfaceType
                                ) {
                                    // interfaceObject: [classObject, metaObject]
                                    Implementation impl = program.implementations[(interfaceType, classType)];
                                    UndertaleVariable variable = metaObjectVariables[impl];
                                    Load(variable, VariableType.Normal);

                                    CallBuiltin("@@NewGMLArray@@", 2);
                                }
                                break;
                            }
                            case PopInstruction: {
                                PopUnused();
                                break;
                            }
                            case DuplicateInstruction duplicate: {
                                // dup 指令似乎不能针对 variable，使用隐藏量。
                                UseHiddenVariables(duplicate.types.Length, variables => {
                                    for (int i = variables.Length - 1; i != -1; --i) {
                                        Store(variables[i], VariableType.Normal);
                                    }
                                    foreach (UndertaleVariable variable in variables) {
                                        Load(variable, VariableType.Normal);
                                    }
                                    foreach (UndertaleVariable variable in variables) {
                                        Load(variable, VariableType.Normal);
                                    }
                                });
                                break;
                            }
                            case JumpInstruction jump: {
                                uint start = byteCount;
                                UndertaleInstruction branch = Branch();
                                SetBranchPromise(branch, jump.index, start);
                                break;
                            }
                            case JumpTrueInstruction jumpTrue: {
                                uint start = byteCount;
                                UndertaleInstruction branch = BranchTrue();
                                SetBranchPromise(branch, jumpTrue.index, start);
                                break;
                            }
                            case JumpFalseInstruction jumpFalse: {
                                uint start = byteCount;
                                UndertaleInstruction branch = BranchFalse();
                                SetBranchPromise(branch, jumpFalse.index, start);
                                break;
                            }
                            case ReturnInstruction ret: {
                                if (!IsZeroSized(ret.type)) {
                                    Return();
                                } else {
                                    Exit();
                                }
                                break;
                            }
                            case StoreElementInstruction: {
                                // push 指令不会修改原件，使用 array_set。
                                CallBuiltin("array_set", 3);
                                PopUnused();
                                break;
                            }
                            case LoadElementInstruction: {
                                // (array, index) -> (LOCAL, index)
                                UseHiddenVariables(2, variables => {
                                    UndertaleVariable indexVariable = variables[0];
                                    UndertaleVariable arrayVariable = variables[1];

                                    Store(indexVariable, VariableType.Normal);
                                    Store(arrayVariable, VariableType.Normal);

                                    PushShort((short)InstanceType.Local);
                                    Load(indexVariable, VariableType.Normal);
                                    Convert(DataType.Int32);

                                    Load(arrayVariable, VariableType.Array);
                                });
                                break;
                            }
                            case StoreFieldInstruction storeField: {
                                // (object, value) -> (array, index, value)
                                UseHiddenVariables(1, variables => {
                                    UndertaleVariable variable = variables[0];
                                    Store(variable, VariableType.Normal);

                                    PushInt(GetFieldIndex(storeField.field));
                                    Load(variable, VariableType.Normal);
                                });
                                CallBuiltin("array_set", 3);
                                PopUnused();
                                break;
                            }
                            case LoadFieldInstruction loadField: {
                                // (object,) -> (LOCAL, index)
                                UseHiddenVariables(1, variables => {
                                    UndertaleVariable objectVariable = variables[0];

                                    Store(objectVariable, VariableType.Normal);

                                    PushShort((short)InstanceType.Local);
                                    PushInt(GetFieldIndex(loadField.field));

                                    Load(objectVariable, VariableType.Array);
                                });
                                break;
                            }
                            case MultiplyInstruction: {
                                Multiply();
                                break;
                            }
                            case DivideInstruction: {
                                Divide();
                                break;
                            }
                            case ModuloInstruction: {
                                Modulo();
                                break;
                            }
                            case AddInstruction: {
                                Add();
                                break;
                            }
                            case SubtractInstruction: {
                                Subtract();
                                break;
                            }
                            case LeftShiftInstruction: {
                                LeftShift();
                                break;
                            }
                            case RightShiftInstruction: {
                                RightShift();
                                break;
                            }
                            case BitAndInstruction: {
                                BitAnd();
                                break;
                            }
                            case BitXorInstruction: {
                                BitXor();
                                break;
                            }
                            case BitOrInstruction: {
                                BitOr();
                                break;
                            }
                            case GreaterInstruction: {
                                Compare(ComparisonType.GT);
                                break;
                            }
                            case GreaterEqualInstruction: {
                                Compare(ComparisonType.GTE);
                                break;
                            }
                            case LessInstruction: {
                                Compare(ComparisonType.LT);
                                break;
                            }
                            case LessEqualInstruction: {
                                Compare(ComparisonType.LTE);
                                break;
                            }
                            case EqualInstruction: {
                                Compare(ComparisonType.EQ);
                                break;
                            }
                            case NotEqualInstruction: {
                                Compare(ComparisonType.NEQ);
                                break;
                            }
                            default:
                                throw new NotImplementedException();
                        }

                        if (instructionOffsetPromises.TryGetValue(i + 1, out var promises)) {
                            foreach ((uint offset, UndertaleInstruction inst) in promises) {
                                inst.JumpOffset = (int)(byteCount - offset) / 4;
                            }
                        }
                    }

                    code!.Replace(instructions);
                    code.Length = byteCount;
                    code.Offset = 0;
                    code.ArgumentsCount = 0;
                    code.LocalsCount = nextLocalIndex;

                    nextLocalIndex = 1;
                    parameterVariables.Clear();
                    instructions.Clear();
                    instructionOffsets.Clear();
                    localVariables.Clear();
                    hiddenLocalVariables.Clear();
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        nextGlobalIndex = 0;
        nextFunctionIndex = 0;
        this.funCodes.Clear();
        this.data = null;
        stringIds.Clear();
        fieldIndices.Clear();
        selfVariables.Clear();
        metaObjectVariables.Clear();
        userFunctions.Clear();
    }
}
