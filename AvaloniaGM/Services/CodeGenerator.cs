using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TypeScript;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.Item;
using TypeScript.Parameter;
using TypeScript.Pattern;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.Type;
using Underanalyzer.Compiler;
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
    readonly Dictionary<FunctionSymbol, UserFunction> userFunctions = [];
    uint nextFunctionIndex = 0;

    readonly Stack<FunctionGenerationState> functionGenerationStates = [];
    uint byteCount;
    List<UndertaleInstruction> instructions = [];
    Stack<DataType> types = [];
    // 这个 1 是 arguments
    uint nextLocalIndex = 1;
    uint nextGlobalIndex = 0;
    uint nextParameterIndex = 0;
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

    void PopUnused() {
        DataType type = types.Pop();
        instructions.Add(new() {
            Kind = Opcode.Popz,
            Type1 = type
        });
        byteCount += 4;
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

    void PushBoolean(bool value) {
        instructions.Add(new() {
            Kind = Opcode.PushI,
            Type1 = DataType.Int16,
            Type2 = DataType.Double,
            ValueShort = value ? (short)1 : (short)0
        });
        byteCount += 4;

        types.Push(DataType.Boolean);
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

    void Negate() {
        DataType type = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Neg,
            Type1 = type
        });
        byteCount += 4;

        types.Push(type);
    }

    void Not() {
        Convert(DataType.Boolean);
        types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Not,
            Type1 = DataType.Boolean
        });
        byteCount += 4;

        types.Push(DataType.Boolean);
    }

    void BitNot() {
        DataType type = types.Peek();
        DataType targetType;
        if (type == DataType.Variable) {
            Convert(DataType.Int64);
            targetType = DataType.Int64;
        } else {
            targetType = type;
        }
        types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Not,
            Type1 = type
        });
        byteCount += 4;

        types.Push(targetType);
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

    void BitAnd() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.And,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else {
            throw new Exception("wtf");
        }
    }

    void BitXor() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Xor,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else {
            throw new Exception("wtf");
        }
    }

    void BitOr() {
        DataType rightType = types.Pop();
        DataType leftType = types.Pop();

        instructions.Add(new() {
            Kind = Opcode.Or,
            Type1 = rightType,
            Type2 = leftType
        });
        byteCount += 4;

        if (leftType == DataType.Int32 && rightType == DataType.Int32) {
            types.Push(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            types.Push(DataType.Int64);
        } else {
            throw new Exception("wtf");
        }
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

    UndertaleVariable AddParameterVariable(IValueSymbol symbol) {
        uint index = nextParameterIndex;
        ++nextParameterIndex;

        UndertaleVariable variable = GetSelfVariable($"argument{index}", true);

        variables.Add(symbol, variable);

        return variable;
    }

    UndertaleVariable AddLocalVariable(string name) {
        (UndertaleString gameString, int id) = GetString(name);

        uint variableId = nextLocalIndex;
        ++nextLocalIndex;

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

    UndertaleVariable GetGlobalVariable(string prefix, bool builtin) {
        string name = $"{prefix}_{nextGlobalIndex}";
        ++nextGlobalIndex;
        (UndertaleString gameString, int id) = GetString(name);

        if (!globalVariables.TryGetValue(name, out UndertaleVariable? result)) {
            result = data!.Variables.Define(gameString, id, InstanceType.Global, builtin, data);
            globalVariables.Add(name, result);
        }

        return result;
    }

    UndertaleVariable GetSelfVariable(string name, bool builtin) {
        (UndertaleString gameString, int id) = GetString(name);

        if (!selfVariables.TryGetValue(name, out UndertaleVariable? result)) {
            result = data!.Variables.Define(gameString, id, InstanceType.Self, builtin, data);
            selfVariables.Add(name, result);
        }

        return result;
    }

    void EnsureClassTypeDefined(ClassType type) {
        if (classTypes.Add(type)) {
            short index = 0;
            foreach (StructField field in type.fields) {
                fieldIndices.Add(field, index);
                ++index;
            }
        }
    }

    short GetFieldIndex(ClassType type, StructField field) {
        EnsureClassTypeDefined(type);
        return fieldIndices[field];
    }

    void StoreSymbol(IValueSymbol symbol) {
        switch (symbol) {
            case LocalSymbol localSymbol: {
                UndertaleVariable variable = variables.GetValueOrDefault(localSymbol)!;
                Store(variable, VariableType.Normal);
                break;
            }
            case ParameterSymbol parameterSymbol: {
                UndertaleVariable variable = variables.GetValueOrDefault(parameterSymbol)!;
                Store(variable, VariableType.Normal);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void LoadSymbol(IValueSymbol symbol) {
        switch (symbol) {
            case FunctionSymbol functionSymbol: {
                if (!userFunctions.TryGetValue(functionSymbol, out UserFunction? userFun)) {
                    throw new Exception("不能求值非自定义函数");
                }
                PushInt(userFun.scriptId);
                break;
            }
            case LocalSymbol localSymbol: {
                UndertaleVariable variable = variables.GetValueOrDefault(localSymbol)!;
                Load(variable, VariableType.Normal);
                break;
            }
            case ParameterSymbol parameterSymbol: {
                UndertaleVariable variable = variables.GetValueOrDefault(parameterSymbol)!;
                Load(variable, VariableType.Normal);
                break;
            }
            case ThisParameterSymbol thisParameterSymbol: {
                UndertaleVariable variable = variables.GetValueOrDefault(thisParameterSymbol)!;
                Load(variable, VariableType.Normal);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void CallBuiltin(string name, int parameterCount) {
        Call(data!.Functions.EnsureDefined(name, data.Strings), parameterCount);
    }

    void Call(UndertaleFunction fun, int parameterCount) {
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

    void CallSymbol(IValueSymbol valueSymbol, int parameterCount) {
        switch (valueSymbol) {
            case FunctionSymbol functionSymbol: {
                UndertaleFunction fun;
                if (functionSymbol.hostName != null) {
                    fun = data!.Functions.EnsureDefined(functionSymbol.hostName, data.Strings);
                } else {
                    fun = GetUserFunction(functionSymbol).fun;
                }
                Call(fun, parameterCount);
                break;
            }
            case LocalSymbol localSymbol: {
                LoadSymbol(localSymbol);
                Convert(DataType.Variable);
                CallBuiltin("script_execute", parameterCount + 1);
                break;
            }
            case ParameterSymbol parameterSymbol: {
                LoadSymbol(parameterSymbol);
                Convert(DataType.Variable);
                CallBuiltin("script_execute", parameterCount + 1);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void PatternAddLocal(IPattern pattern, IExpression? initializer) {
        switch (pattern) {
            case IdentifierPattern identifierPattern: {
                var symbol = (LocalSymbol)identifierPattern.GetSymbol();

                UndertaleVariable variable = AddLocalVariable(identifierPattern.name);
                variables.Add(symbol, variable);

                if (initializer != null) {
                    EvaluateExpression(initializer, symbol.type);
                    Store(variable, VariableType.Normal);
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void PatternAddParameter(IPattern pattern) {
        switch (pattern) {
            case IdentifierPattern identifierPattern: {
                AddParameterVariable((ParameterSymbol)identifierPattern.GetSymbol());
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ExpressionCall(IExpression called, int parameterCount) {
        switch (called) {
            case PathExpression pathExpression: {
                CallSymbol(pathExpression.GetSymbol(), parameterCount);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void NegateByType(IType type) {
        switch (type) {
            case IntegerType: {
                Negate();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void NotByType(IType type) {
        switch (type) {
            case BooleanType: {
                Not();
                break;
            }
            case IntegerType: {
                throw new NotImplementedException();
            }
            default:
                throw new NotImplementedException();
        }
    }

    void SetIndexByType(IType type, IExpression indexed, IExpression index, IExpression value) {
        switch (type) {
            case ArrayType arrayType: {
                UndertaleVariable variable = EvaluateExpressionToVariable(indexed, "indexed");
                EvaluateExpression(value, arrayType.elementType);

                PushShort((short)variable.InstanceType);
                EvaluateExpression(index, null);
                Convert(DataType.Int32);

                Store(variable, VariableType.Array);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void SetFieldByType(IType type, IExpression accessed, IField field, IExpression value) {
        switch (field) {
            case StructField structField: {
                UndertaleVariable variable = EvaluateExpressionToVariable(accessed, "accessed");
                EvaluateExpression(value, structField.type);

                PushShort((short)variable.InstanceType);
                short index = GetFieldIndex((ClassType)type, structField);
                PushShort(index);
                Convert(DataType.Int32);

                Store(variable, VariableType.Array);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void GetIndexByType(IType type, IExpression indexed, IExpression index) {
        switch (type) {
            case ArrayType: {
                UndertaleVariable variable = EvaluateExpressionToVariable(indexed, "indexed");

                PushShort((short)variable.InstanceType);
                EvaluateExpression(index, null);
                Convert(DataType.Int32);

                Load(variable, VariableType.Array);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void GetFieldByType(IType type, IExpression accessed, IField field) {
        switch (field) {
            case StructField structField: {
                UndertaleVariable variable = EvaluateExpressionToVariable(accessed, "accessed");

                PushShort((short)variable.InstanceType);
                short index = GetFieldIndex((ClassType)type, structField);
                PushShort(index);
                Convert(DataType.Int32);

                Load(variable, VariableType.Array);
                break;
            }
            case ArrayLengthField: {
                EvaluateExpression(accessed, null);
                CallBuiltin("array_length_1d", 1);
                break;
            }
            case StringLengthField: {
                EvaluateExpression(accessed, null);
                CallBuiltin("string_length", 1);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void CallMethodByType(IType type, IExpression callee, string name, IExpression[] parameters) {
        switch (type) {
            case ClassType classType: {
                var symbol = (FunctionSymbol)classType.GetSymbol(name);
                var funType = symbol.GetFunctionType();

                for (int i = parameters.Length - 1; i != -1; --i) {
                    EvaluateExpression(parameters[i], funType.parameterTypes[i + 1]);
                    Convert(DataType.Variable);
                }
                EvaluateExpression(callee, null);
                Convert(DataType.Variable);

                CallSymbol(symbol, parameters.Length + 1);
                break;
            }
            case InterfaceType interfaceType: {
                // interfaceObject.methods[methodIndex](interfaceObject.classObject, parameter0, parameter1...)
                ConceptFunctionSymbol symbol = interfaceType.GetConceptFunction(name);
                IType[] parameterTypes = symbol.GetParameterTypes();
                for (int i = parameters.Length - 1; i != -1; --i) {
                    EvaluateExpression(parameters[i], parameterTypes[i]);
                    Convert(DataType.Variable);
                }
                UndertaleVariable calleeVariable = EvaluateExpressionToVariable(callee, "callee");

                // 载入方法表
                UndertaleVariable methodsVariable = AddLocalVariable("methods");

                PushShort((short)calleeVariable.InstanceType);
                PushShort(1);
                Load(calleeVariable, VariableType.Array);
                Store(methodsVariable, VariableType.Normal);

                // 载入 classObject
                PushShort((short)calleeVariable.InstanceType);
                PushShort(0);
                Load(calleeVariable, VariableType.Array);

                // 载入实际方法
                PushShort((short)methodsVariable.InstanceType);
                PushInt((int)symbol.index);
                Load(methodsVariable, VariableType.Array);

                CallBuiltin("script_execute", parameters.Length + 2);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void MultiplyByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Multiply();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void DivideByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Divide();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ModuloByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Modulo();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void AddByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Add();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void SubtractByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Subtract();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void LeftShiftByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                throw new NotImplementedException();
            }
            default:
                throw new NotImplementedException();
        }
    }

    void RightShiftByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                throw new NotImplementedException();
            }
            default:
                throw new NotImplementedException();
        }
    }

    void BitAndByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                Convert(DataType.Int32);
                EvaluateExpression(right, null);
                Convert(DataType.Int32);
                BitAnd();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void BitXorByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                Convert(DataType.Int32);
                EvaluateExpression(right, null);
                Convert(DataType.Int32);
                BitXor();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void BitOrByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                Convert(DataType.Int32);
                EvaluateExpression(right, null);
                Convert(DataType.Int32);
                BitOr();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void GreaterByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Compare(ComparisonType.GT);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void GreaterEqualByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Compare(ComparisonType.GTE);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void LessByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Compare(ComparisonType.LT);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void LessEqualByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Compare(ComparisonType.LTE);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void EqualByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Compare(ComparisonType.EQ);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void NotEqualByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Compare(ComparisonType.NEQ);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void AssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case IndexExpression indexExpression: {
                SetIndexByType(indexExpression.indexed.GetResultType(), indexExpression.indexed, indexExpression.index, expression);
                break;
            }
            case FieldAccessExpression accessExpr: {
                SetFieldByType(accessExpr.accessed.GetResultType(), accessExpr.accessed, accessExpr.GetField(), expression);
                break;
            }
            case PathExpression pathExpression: {
                EvaluateExpression(expression, placeExpression.GetResultType());
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void AddAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                AddAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void SubtractAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                SubtractAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void MultiplyAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                MultiplyAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void DivideAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                DivideAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void ModuloAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                ModuloAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void BitAndAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                BitAndAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void BitOrAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                BitOrAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void BitXorAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                BitXorAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void LeftShiftAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                LeftShiftAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void RightShiftAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression, null);
                RightShiftAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void AddAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType:
            case StringType: {
                EvaluateExpression(right, null);
                Add();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void SubtractAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Subtract();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void MultiplyAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Multiply();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void DivideAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Divide();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ModuloAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right, null);
                Modulo();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void BitAndAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                Convert(DataType.Int32);
                EvaluateExpression(right, null);
                Convert(DataType.Int32);
                BitAnd();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void BitOrAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                Convert(DataType.Int32);
                EvaluateExpression(right, null);
                Convert(DataType.Int32);
                BitOr();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void BitXorAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                Convert(DataType.Int32);
                EvaluateExpression(right, null);
                Convert(DataType.Int32);
                BitXor();
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void LeftShiftAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                throw new NotImplementedException();
            }
            default:
                throw new NotImplementedException();
        }
    }

    void RightShiftAssignByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                throw new NotImplementedException();
            }
            default:
                throw new NotImplementedException();
        }
    }

    UndertaleVariable EvaluateExpressionToVariable(IExpression expression, string name) {
        switch (expression) {
            case PathExpression pathExpression: {
                IValueSymbol symbol = pathExpression.GetSymbol();
                return variables.GetValueOrDefault(symbol)!;
            }
            case ArrayExpression arrayExpression: {
                UndertaleVariable variable = AddLocalVariable(name);

                if (arrayExpression.elements.Length != 0) {
                    short elementIndex = 0;
                    foreach (IExpression element in arrayExpression.elements) {
                        EvaluateExpression(element, null);

                        PushShort((short)InstanceType.Local);
                        PushShort(elementIndex);
                        ++elementIndex;

                        Store(variable, VariableType.Array);
                    }
                } else {
                    PushShort(0);
                    Convert(DataType.Variable);
                    CallBuiltin("array_create", 1);

                    Store(variable, VariableType.Normal);
                }

                return variable;
            }
            case NewExpression newExpression: {
                UndertaleVariable variable = AddLocalVariable(name);

                if (newExpression.fields.Length != 0) {
                    foreach (NewExpression.Field field in newExpression.fields) {
                        StructField classField = field.GetField();
                        short fieldIndex = GetFieldIndex(newExpression.GetClassType(), classField);

                        EvaluateExpression(field.value, classField.type);

                        PushShort((short)InstanceType.Local);
                        PushShort(fieldIndex);

                        Store(variable, VariableType.Array);
                    }
                } else {
                    PushShort(0);
                    Convert(DataType.Variable);
                    CallBuiltin("array_create", 1);

                    Store(variable, VariableType.Normal);
                }

                return variable;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void EvaluateExpression(IExpression expression, IType? targetType) {
        switch (expression) {
            case IntegerExpression integerExpression: {
                int value = int.Parse(integerExpression.valueStr);

                instructions.Add(new() {
                    Kind = Opcode.Push,
                    Type1 = DataType.Int32,
                    Type2 = DataType.Double,
                    ValueInt = value
                });
                byteCount += 8;

                types.Push(DataType.Int32);
                break;
            }
            case BooleanExpression booleanExpression: {
                instructions.Add(new() {
                    Kind = Opcode.PushI,
                    Type1 = DataType.Int16,
                    Type2 = DataType.Double,
                    ValueShort = booleanExpression.value ? (short)1 : (short)0
                });
                byteCount += 4;

                types.Push(DataType.Boolean);
                break;
            }
            case StringExpression stringExpression: {
                (UndertaleString gameString, int id) = GetString(stringExpression.value);

                instructions.Add(new() {
                    Kind = Opcode.Push,
                    Type1 = DataType.String,
                    ValueString = new UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>(gameString, id)
                });
                byteCount += 8;

                types.Push(DataType.String);
                break;
            }
            case PathExpression pathExpression: {
                LoadSymbol(pathExpression.GetSymbol());
                break;
            }
            case ArrayExpression arrayExpression: {
                UndertaleVariable variable = EvaluateExpressionToVariable(arrayExpression, "arr");
                Load(variable, VariableType.Normal);
                break;
            }
            case NewExpression newExpression: {
                UndertaleVariable variable = EvaluateExpressionToVariable(newExpression, "obj");
                Load(variable, VariableType.Normal);
                break;
            }
            case IndexExpression indexExpression: {
                GetIndexByType(indexExpression.indexed.GetResultType(), indexExpression.indexed, indexExpression.index);
                break;
            }
            case FieldAccessExpression fieldAccessExpression: {
                GetFieldByType(fieldAccessExpression.accessed.GetResultType(), fieldAccessExpression.accessed, fieldAccessExpression.GetField());
                break;
            }
            case MethodCallExpression methodCallExpression: {
                CallMethodByType(methodCallExpression.callee.GetResultType(), methodCallExpression.callee, methodCallExpression.name, methodCallExpression.parameters);
                break;
            }
            case CallExpression callExpression: {
                IExpression[] parameters = callExpression.parameters;

                IType[] calleeParameterTypes = callExpression.GetCalleeType().parameterTypes;
                for (int i = parameters.Length - 1; i != -1; --i) {
                    EvaluateExpression(parameters[i], calleeParameterTypes[i]);
                    Convert(DataType.Variable);
                }
                ExpressionCall(callExpression.callee, parameters.Length);
                break;
            }
            case NegateExpression negateExpression: {
                EvaluateExpression(negateExpression.expression, null);
                NegateByType(negateExpression.expression.GetResultType());
                break;
            }
            case NotExpression notExpression: {
                EvaluateExpression(notExpression.expression, null);
                NotByType(notExpression.expression.GetResultType());
                break;
            }
            case MultiplyExpression multiplyExpression: {
                EvaluateExpression(multiplyExpression.left, null);
                MultiplyByType(multiplyExpression.left.GetResultType(), multiplyExpression.right);
                break;
            }
            case DivideExpression divideExpression: {
                EvaluateExpression(divideExpression.left, null);
                DivideByType(divideExpression.left.GetResultType(), divideExpression.right);
                break;
            }
            case ModuloExpression moduloExpression: {
                EvaluateExpression(moduloExpression.left, null);
                ModuloByType(moduloExpression.left.GetResultType(), moduloExpression.right);
                break;
            }
            case AddExpression addExpression: {
                EvaluateExpression(addExpression.left, null);
                AddByType(addExpression.left.GetResultType(), addExpression.right);
                break;
            }
            case SubtractExpression subtractExpression: {
                EvaluateExpression(subtractExpression.left, null);
                SubtractByType(subtractExpression.left.GetResultType(), subtractExpression.right);
                break;
            }
            case LeftShiftExpression leftShiftExpression: {
                EvaluateExpression(leftShiftExpression.left, null);
                LeftShiftByType(leftShiftExpression.left.GetResultType(), leftShiftExpression.right);
                break;
            }
            case RightShiftExpression rightShiftExpression: {
                EvaluateExpression(rightShiftExpression.left, null);
                RightShiftByType(rightShiftExpression.left.GetResultType(), rightShiftExpression.right);
                break;
            }
            case BitAndExpression bitAndExpression: {
                EvaluateExpression(bitAndExpression.left, null);
                BitAndByType(bitAndExpression.left.GetResultType(), bitAndExpression.right);
                break;
            }
            case BitXorExpression bitXorExpression: {
                EvaluateExpression(bitXorExpression.left, null);
                BitXorByType(bitXorExpression.left.GetResultType(), bitXorExpression.right);
                break;
            }
            case BitOrExpression bitOrExpression: {
                EvaluateExpression(bitOrExpression.left, null);
                BitOrByType(bitOrExpression.left.GetResultType(), bitOrExpression.right);
                break;
            }
            case GreaterExpression greaterExpression: {
                EvaluateExpression(greaterExpression.left, null);
                GreaterByType(greaterExpression.left.GetResultType(), greaterExpression.right);
                break;
            }
            case GreaterEqualExpression greaterEqualExpression: {
                EvaluateExpression(greaterEqualExpression.left, null);
                GreaterEqualByType(greaterEqualExpression.left.GetResultType(), greaterEqualExpression.right);
                break;
            }
            case LessExpression lessExpression: {
                EvaluateExpression(lessExpression.left, null);
                LessByType(lessExpression.left.GetResultType(), lessExpression.right);
                break;
            }
            case LessEqualExpression lessEqualExpression: {
                EvaluateExpression(lessEqualExpression.left, null);
                LessEqualByType(lessEqualExpression.left.GetResultType(), lessEqualExpression.right);
                break;
            }
            case EqualExpression equalExpression: {
                EvaluateExpression(equalExpression.left, null);
                EqualByType(equalExpression.left.GetResultType(), equalExpression.right);
                break;
            }
            case NotEqualExpression notEqualExpression: {
                EvaluateExpression(notEqualExpression.left, null);
                NotEqualByType(notEqualExpression.left.GetResultType(), notEqualExpression.right);
                break;
            }
            case AndExpression andExpression: {
                EvaluateExpression(andExpression.left, null);
                Convert(DataType.Boolean);

                uint bfStart = byteCount;
                UndertaleInstruction bfInstruction = BranchFalse();

                EvaluateExpression(andExpression.right, null);
                Convert(DataType.Boolean);

                uint branchStart = byteCount;
                UndertaleInstruction branchInstruction = Branch();

                bfInstruction.JumpOffset = (int)(byteCount - bfStart) / 4;
                PushBoolean(false);

                branchInstruction.JumpOffset = (int)(byteCount - branchStart) / 4;
                break;
            }
            case OrExpression orExpression: {
                EvaluateExpression(orExpression.left, null);
                Convert(DataType.Boolean);

                uint btStart = byteCount;
                UndertaleInstruction btInstruction = BranchTrue();

                EvaluateExpression(orExpression.right, null);
                Convert(DataType.Boolean);

                uint branchStart = byteCount;
                UndertaleInstruction branchInstruction = Branch();

                btInstruction.JumpOffset = (int)(byteCount - btStart) / 4;
                PushBoolean(false);

                branchInstruction.JumpOffset = (int)(byteCount - branchStart) / 4;
                break;
            }
            case AssignExpression assignExpression: {
                AssignToPlace(assignExpression.left, assignExpression.right);
                break;
            }
            case AddAssignExpression addAssignExpression: {
                AddAssignToPlace(addAssignExpression.left, addAssignExpression.right);
                break;
            }
            case SubtractAssignExpression subtractAssignExpression: {
                SubtractAssignToPlace(subtractAssignExpression.left, subtractAssignExpression.right);
                break;
            }
            case MultiplyAssignExpression multiplyAssignExpression: {
                MultiplyAssignToPlace(multiplyAssignExpression.left, multiplyAssignExpression.right);
                break;
            }
            case DivideAssignExpression divideAssignExpression: {
                DivideAssignToPlace(divideAssignExpression.left, divideAssignExpression.right);
                break;
            }
            case ModuloAssignExpression moduloAssignExpression: {
                ModuloAssignToPlace(moduloAssignExpression.left, moduloAssignExpression.right);
                break;
            }
            case BitAndAssignExpression bitAndAssignExpression: {
                BitAndAssignToPlace(bitAndAssignExpression.left, bitAndAssignExpression.right);
                break;
            }
            case BitOrAssignExpression bitOrAssignExpression: {
                BitOrAssignToPlace(bitOrAssignExpression.left, bitOrAssignExpression.right);
                break;
            }
            case BitXorAssignExpression bitXorAssignExpression: {
                BitXorAssignToPlace(bitXorAssignExpression.left, bitXorAssignExpression.right);
                break;
            }
            case LeftShiftAssignExpression leftShiftAssignExpression: {
                LeftShiftAssignToPlace(leftShiftAssignExpression.left, leftShiftAssignExpression.right);
                break;
            }
            case RightShiftAssignExpression rightShiftAssignExpression: {
                RightShiftAssignToPlace(rightShiftAssignExpression.left, rightShiftAssignExpression.right);
                break;
            }
            default:
                throw new NotImplementedException();
        }

        if (expression.GetResultType() is ClassType classType && targetType is InterfaceType interfaceType) {
            // 处理 class 对象转换成接口对象
            UndertaleVariable variable = AddLocalVariable("tempInter");

            PushShort((short)variable.InstanceType);
            PushShort(0);
            Store(variable, VariableType.Array);

            UndertaleVariable methodsVariable = methodsVariables[(classType, interfaceType)];
            Load(methodsVariable, VariableType.Normal);
            PushShort((short)variable.InstanceType);
            PushShort(1);
            Store(variable, VariableType.Array);

            Load(variable, VariableType.Normal);
        }
    }

    void ExecuteStatement(IStatement statement) {
        switch (statement) {
            case ExpressionStatement expressionStatement: {
                EvaluateExpression(expressionStatement.expression, null);
                // 特殊处理：调用必定返回 Variable
                if (
                    expressionStatement.expression is CallExpression
                    || expressionStatement.expression is MethodCallExpression
                    || expressionStatement.expression.GetResultType() != TupleType.EMPTY
                ) {
                    PopUnused();
                }
                break;
            }
            case LetStatement letStatement: {
                PatternAddLocal(letStatement.pattern, letStatement.initializer);
                break;
            }
            case IfStatement ifStatement: {
                EvaluateExpression(ifStatement.condition, null);
                uint bfStart = byteCount;
                UndertaleInstruction bfInstruction = BranchFalse();

                ExecuteBlockStatement(ifStatement.body);

                if (ifStatement.elseBody != null) {
                    uint branchStart = byteCount;
                    UndertaleInstruction branchInstruction = Branch();

                    bfInstruction.JumpOffset = (int)(byteCount - bfStart) / 4;

                    ExecuteStatement(ifStatement.elseBody);

                    branchInstruction.JumpOffset = (int)(byteCount - branchStart) / 4;
                } else {
                    bfInstruction.JumpOffset = (int)(byteCount - bfStart) / 4;
                }
                break;
            }
            case WhileStatement whileStatement: {
                uint start = byteCount;
                LoopContext context = new(start);

                EvaluateExpression(whileStatement.condition, null);
                uint bfStart = byteCount;
                UndertaleInstruction bfInstruction = BranchFalse();

                loopContexts.Add(whileStatement, context);
                ExecuteBlockStatement(whileStatement.body);

                uint branchStart = byteCount;
                UndertaleInstruction branchInstruction = Branch();
                branchInstruction.JumpOffset = (int)(start - branchStart) / 4;

                uint endOffset = byteCount;
                bfInstruction.JumpOffset = (int)(endOffset - bfStart) / 4;
                foreach ((uint instructionStart, UndertaleInstruction instruction) in context.IteratePromise()) {
                    instruction.JumpOffset = (int)(endOffset - instructionStart) / 4;
                }

                loopContexts.Remove(whileStatement);
                break;
            }
            case ContinueStatement continueStatement: {
                uint loopStart = loopContexts[continueStatement.GetLoop()].start;
                uint branchStart = byteCount;
                UndertaleInstruction branch = Branch();
                branch.JumpOffset = (int)(loopStart - branchStart) / 4;
                break;
            }
            case BreakStatement breakStatement: {
                uint branchStart = byteCount;
                UndertaleInstruction branch = Branch();
                loopContexts[breakStatement.GetLoop()].AddPromise(branchStart, branch);
                break;
            }
            case ReturnStatement returnStatement: {
                if (returnStatement.value != null) {
                    EvaluateExpression(returnStatement.value, returnStatement.GetTargetType());
                    if (returnStatement.value.GetResultType() != TupleType.EMPTY) {
                        Return();
                    } else {
                        Exit();
                    }
                } else {
                    Exit();
                }
                break;
            }
            case BlockStatement blockStatement: {
                ExecuteBlockStatement(blockStatement);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ExecuteBlockStatement(BlockStatement blockStatement) {
        foreach (IItem item in blockStatement.items) {
            GenerateItem(item);
        }
        foreach (ImplNode impl in blockStatement.impls) {
            GenerateImpl(impl);
        }
        foreach (IStatement statement in blockStatement.statements) {
            ExecuteStatement(statement);
        }
    }

    void AddParameterVariableByType(IParameter parameter) {
        switch (parameter) {
            case PatternParameter patternParameter: {
                PatternAddParameter(patternParameter.pattern);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    UserFunction GetUserFunction(FunctionSymbol symbol) {
        if (!userFunctions.TryGetValue(symbol, out UserFunction? result)) {
            string funName = $"{symbol.name}_{nextFunctionIndex}";
            ++nextFunctionIndex;

            var code = UndertaleCode.CreateEmptyEntry(data, $"gml_Script_{funName}");
            UndertaleString nameStr = data!.Strings.MakeString(funName);
            int scriptId = data.Scripts.Count;
            UndertaleScript script = new() {
                Name = nameStr,
                Code = code
            };

            data.Scripts.Add(script);

            UndertaleFunction fun = data.Functions.EnsureDefined(funName, data.Strings);

            result = new(scriptId, script, fun, code);
            userFunctions.Add(symbol, result);
        }

        return result;
    }

    void GenerateImpl(ImplNode impl) {
        foreach (FunctionItem funItem in impl.functions) {
            GenerateFunctionItem(funItem);
        }
    }

    void GenerateFunctionItem(FunctionItem funItem) {
        if (funItem.body == null || funItem.hostName != null) {
            return;
        }

        functionGenerationStates.Push(new(byteCount, instructions, types, nextLocalIndex, nextParameterIndex, codeLocals!, loopContexts));
        byteCount = 0;
        instructions = [];
        types = [];
        nextLocalIndex = 1;
        nextParameterIndex = 0;

        UndertaleCode? code = funCodes.GetValueOrDefault(funItem.name);
        if (code == null) {
            UserFunction userFun = GetUserFunction(funItem.GetSymbol());
            code = userFun.code;
        } else {
            // 静态期：生成方法表
            if (funItem.body != null) {
                foreach (IItem blockItem in funItem.body.items) {
                    GenerateItemStatic(blockItem);
                }
                foreach (ImplNode impl in funItem.body.impls) {
                    GenerateImplStatic(impl);
                }
            }
        }
        codeLocals = data!.CodeLocals.For(code);

        if (funItem.thisParameter != null) {
            AddParameterVariable(funItem.thisParameter.GetSymbol());
        }
        foreach (IParameter parameter in funItem.parameters) {
            AddParameterVariableByType(parameter);
        }

        ExecuteBlockStatement(funItem.body!);

        code.Replace(instructions);
        code.Length = byteCount;
        code.Offset = 0;
        code.ArgumentsCount = 0;
        code.LocalsCount = nextLocalIndex;

        FunctionGenerationState state = functionGenerationStates.Pop();
        byteCount = state.byteCount;
        instructions = state.instructions;
        types = state.types;
        nextLocalIndex = state.nextLocalIndex;
        nextParameterIndex = state.nextParameterIndex;
    }

    void GenerateItem(IItem item) {
        switch (item) {
            case FunctionItem funItem: {
                GenerateFunctionItem(funItem);
                break;
            }
            case ClassItem: {
                // 无事发生，因为字段表是惰性添加的
                break;
            }
            case InterfaceItem: {
                // 无事发生
                break;
            }
            case NameSpaceItem nameSpaceItem: {
                foreach (IItem subitem in nameSpaceItem.items) {
                    GenerateItem(subitem);
                }
                foreach (ImplNode impl in nameSpaceItem.impls) {
                    GenerateImpl(impl);
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void GenerateImplStatic(ImplNode impl) {
        if (impl.methodTable != null) {
            UndertaleVariable variable = GetGlobalVariable("methods", false);

            short index = 0;
            foreach (FunctionSymbol symbol in impl.methodTable) {
                UserFunction userFun = GetUserFunction(symbol);
                PushInt(userFun.scriptId);

                PushShort((short)variable.InstanceType);
                PushShort(index);

                Store(variable, VariableType.Array);
            }

            methodsVariables.Add((impl.GetImplementedType(), impl.GetRegardedType()), variable);
        }
    }

    void GenerateItemStatic(IItem item) {
        switch (item) {
            case FunctionItem funItem: {
                if (funItem.body != null) {
                    foreach (IItem blockItem in funItem.body.items) {
                        GenerateItemStatic(blockItem);
                    }
                    foreach (ImplNode impl in funItem.body.impls) {
                        GenerateImplStatic(impl);
                    }
                }
                break;
            }
            case ClassItem:
            case InterfaceItem: {
                // 无事发生
                break;
            }
            case NameSpaceItem nameSpaceItem: {
                foreach (IItem subitem in nameSpaceItem.items) {
                    GenerateItemStatic(subitem);
                }
                foreach (ImplNode impl in nameSpaceItem.impls) {
                    GenerateImplStatic(impl);
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    internal void Generate(UndertaleData data, FileNode[] nodes, Dictionary<string, UndertaleCode> funCodes) {
        this.data = data;
        foreach ((string name, UndertaleCode code) in funCodes) {
            this.funCodes.Add(name, code);
        }

        foreach (FileNode node in nodes) {
            foreach (IItem item in node.items) {
                GenerateItem(item);
            }
            foreach (ImplNode impl in node.impls) {
                GenerateImpl(impl);
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
