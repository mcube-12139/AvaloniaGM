using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TypeScript;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Pattern;
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
    UndertaleCode? nowCode;
    UndertaleCodeLocals? codeLocals;
    readonly Dictionary<string, int> stringIds = [];
    readonly Dictionary<VariableSymbol, (UndertaleVariable variable, VariableType variableType)> variables = [];
    readonly Dictionary<ILoopStatement, LoopContext> breakPromises = [];
    uint byteCount;
    // 这个 1 是 arguments
    uint nextLocalIndex = 1;
    readonly Stack<DataType> types = [];
    List<UndertaleInstruction> instructions = [];

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

    void PushType(DataType type) {
        types.Push(type);
    }

    void PopType() {
        types.Pop();
    }

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

        PushType(target);
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

        PushType(DataType.Boolean);
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

        PopType();

        return result;
    }

    UndertaleInstruction BranchFalse() {
        UndertaleInstruction result = new() {
            Kind = Opcode.Bf
        };

        instructions.Add(result);
        byteCount += 4;

        PopType();

        return result;
    }

    void PushBoolean(bool value) {
        instructions.Add(new() {
            Kind = Opcode.PushI,
            Type1 = DataType.Int16,
            Type2 = DataType.Double,
            ValueShort = value ? (short)1 : (short)0
        });
        byteCount += 4;

        PushType(DataType.Boolean);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            PushType(DataType.Double);
        } else {
            PushType(DataType.Variable);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            PushType(DataType.Double);
        } else {
            PushType(DataType.Variable);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            PushType(DataType.Double);
        } else {
            PushType(DataType.Variable);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
        } else if (leftType == DataType.String && rightType == DataType.String) {
            PushType(DataType.String);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            PushType(DataType.Double);
        } else {
            PushType(DataType.Variable);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
        } else if (leftType == DataType.Double && rightType == DataType.Double) {
            PushType(DataType.Double);
        } else {
            PushType(DataType.Variable);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
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
            PushType(DataType.Int32);
        } else if (leftType == DataType.Int64 && rightType == DataType.Int64) {
            PushType(DataType.Int64);
        } else {
            throw new Exception("wtf");
        }
    }

    void Store(UndertaleVariable variable, VariableType variableType) {
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

        if (variableType == VariableType.Array) {
            PopType();
            PopType();
        }
    }

    void Load(UndertaleVariable variable, VariableType variableType) {
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

        if (variableType == VariableType.Array) {
            PopType();
            PopType();
        }

        PushType(DataType.Variable);
    }

    void StoreSymbol(IValueSymbol symbol) {
        switch (symbol) {
            case VariableSymbol variableSymbol: {
                (UndertaleVariable variable, VariableType variableType) = variables.GetValueOrDefault(variableSymbol);
                Store(variable, variableType);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void LoadSymbol(IValueSymbol symbol) {
        switch (symbol) {
            case VariableSymbol variableSymbol: {
                (UndertaleVariable variable, VariableType variableType) = variables.GetValueOrDefault(variableSymbol);
                Load(variable, variableType);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void Call(UndertaleFunction fun, int parameterCount) {
        for (int i = parameterCount; i != 0; --i) {
            PopType();
        }

        instructions.Add(new() {
            Kind = Opcode.Call,
            Type1 = DataType.Int32,
            ArgumentsCount = (ushort)parameterCount,
            ValueFunction = fun
        });
        byteCount += 8;

        PushType(DataType.Variable);
    }

    void CallSymbol(IValueSymbol valueSymbol) {
        switch (valueSymbol) {
            case FunctionSymbol functionSymbol: {
                if (functionSymbol.hostName != null) {
                    UndertaleFunction fun = data!.Functions.EnsureDefined(functionSymbol.hostName, data.Strings);
                    Call(fun, functionSymbol.GetSharkType().parameterTypes.Length);
                } else {
                    throw new NotImplementedException();
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void PatternAddVariable(IPattern pattern, IExpression? initializer) {
        switch (pattern) {
            case IdentifierPattern identifierPattern: {
                (UndertaleString gameString, int id) = GetString(identifierPattern.name);
                uint variableId = nextLocalIndex;
                ++nextLocalIndex;

                UndertaleVariable variable = data!.Variables.DefineLocal(data, (int)variableId, gameString, id);
                codeLocals!.Locals.Add(new() {
                    Index = (uint)id,
                    Name = gameString,
                });
                variables.Add(identifierPattern.GetSymbol(), (variable, VariableType.Normal));

                if (nextLocalIndex > data.MaxLocalVarCount) {
                    data.MaxLocalVarCount = nextLocalIndex;
                }

                if (initializer != null) {
                    EvaluateExpression(initializer);
                    Store(variable, VariableType.Normal);
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ExpressionCall(IExpression called) {
        switch (called) {
            case PathExpression pathExpression: {
                CallSymbol(pathExpression.GetSymbol());
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void MultiplyByType(IType type, IExpression right) {
        switch (type) {
            case IntegerType: {
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
                Compare(ComparisonType.NEQ);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void AssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void AddAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                AddAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void SubtractAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                SubtractAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void MultiplyAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                MultiplyAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void DivideAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                DivideAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void ModuloAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                ModuloAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void BitAndAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                BitAndAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void BitOrAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                BitOrAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void BitXorAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                BitXorAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void LeftShiftAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
                LeftShiftAssignByType(placeExpression.GetResultType(), expression);
                StoreSymbol(pathExpression.GetSymbol());
                break;
            }
        }
    }

    void RightShiftAssignToPlace(IPlaceExpression placeExpression, IExpression expression) {
        switch (placeExpression) {
            case PathExpression pathExpression: {
                EvaluateExpression(placeExpression);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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
                EvaluateExpression(right);
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

    void EvaluateExpression(IExpression expression) {
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

                PushType(DataType.Int32);
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

                PushType(DataType.String);
                break;
            }
            case PathExpression pathExpression: {
                LoadSymbol(pathExpression.GetSymbol());
                break;
            }
            case CallExpression callExpression: {
                IExpression[] parameters = callExpression.parameters;

                for (int i = parameters.Length - 1; i != -1; --i) {
                    EvaluateExpression(parameters[i]);
                    Convert(DataType.Variable);
                }
                ExpressionCall(callExpression.called);
                break;
            }
            case MultiplyExpression multiplyExpression: {
                EvaluateExpression(multiplyExpression.left);
                MultiplyByType(multiplyExpression.left.GetResultType(), multiplyExpression.right);
                break;
            }
            case DivideExpression divideExpression: {
                EvaluateExpression(divideExpression.left);
                DivideByType(divideExpression.left.GetResultType(), divideExpression.right);
                break;
            }
            case ModuloExpression moduloExpression: {
                EvaluateExpression(moduloExpression.left);
                ModuloByType(moduloExpression.left.GetResultType(), moduloExpression.right);
                break;
            }
            case AddExpression addExpression: {
                EvaluateExpression(addExpression.left);
                AddByType(addExpression.left.GetResultType(), addExpression.right);
                break;
            }
            case SubtractExpression subtractExpression: {
                EvaluateExpression(subtractExpression.left);
                SubtractByType(subtractExpression.left.GetResultType(), subtractExpression.right);
                break;
            }
            case LeftShiftExpression leftShiftExpression: {
                EvaluateExpression(leftShiftExpression.left);
                LeftShiftByType(leftShiftExpression.left.GetResultType(), leftShiftExpression.right);
                break;
            }
            case RightShiftExpression rightShiftExpression: {
                EvaluateExpression(rightShiftExpression.left);
                RightShiftByType(rightShiftExpression.left.GetResultType(), rightShiftExpression.right);
                break;
            }
            case BitAndExpression bitAndExpression: {
                EvaluateExpression(bitAndExpression.left);
                BitAndByType(bitAndExpression.left.GetResultType(), bitAndExpression.right);
                break;
            }
            case BitXorExpression bitXorExpression: {
                EvaluateExpression(bitXorExpression.left);
                BitXorByType(bitXorExpression.left.GetResultType(), bitXorExpression.right);
                break;
            }
            case BitOrExpression bitOrExpression: {
                EvaluateExpression(bitOrExpression.left);
                BitOrByType(bitOrExpression.left.GetResultType(), bitOrExpression.right);
                break;
            }
            case GreaterExpression greaterExpression: {
                EvaluateExpression(greaterExpression.left);
                GreaterByType(greaterExpression.left.GetResultType(), greaterExpression.right);
                break;
            }
            case GreaterEqualExpression greaterEqualExpression: {
                EvaluateExpression(greaterEqualExpression.left);
                GreaterEqualByType(greaterEqualExpression.left.GetResultType(), greaterEqualExpression.right);
                break;
            }
            case LessExpression lessExpression: {
                EvaluateExpression(lessExpression.left);
                LessByType(lessExpression.left.GetResultType(), lessExpression.right);
                break;
            }
            case LessEqualExpression lessEqualExpression: {
                EvaluateExpression(lessEqualExpression.left);
                LessEqualByType(lessEqualExpression.left.GetResultType(), lessEqualExpression.right);
                break;
            }
            case EqualExpression equalExpression: {
                EvaluateExpression(equalExpression.left);
                EqualByType(equalExpression.left.GetResultType(), equalExpression.right);
                break;
            }
            case NotEqualExpression notEqualExpression: {
                EvaluateExpression(notEqualExpression.left);
                NotEqualByType(notEqualExpression.left.GetResultType(), notEqualExpression.right);
                break;
            }
            case AndExpression andExpression: {
                EvaluateExpression(andExpression.left);
                Convert(DataType.Boolean);

                uint bfStart = byteCount;
                UndertaleInstruction bfInstruction = BranchFalse();

                EvaluateExpression(andExpression.right);
                Convert(DataType.Boolean);

                uint branchStart = byteCount;
                UndertaleInstruction branchInstruction = Branch();

                bfInstruction.JumpOffset = (int)(byteCount - bfStart) / 4;
                PushBoolean(false);

                branchInstruction.JumpOffset = (int)(byteCount - branchStart) / 4;
                break;
            }
            case OrExpression orExpression: {
                EvaluateExpression(orExpression.left);
                Convert(DataType.Boolean);

                uint btStart = byteCount;
                UndertaleInstruction btInstruction = BranchTrue();

                EvaluateExpression(orExpression.right);
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
    }

    void ExecuteStatement(IStatement statement) {
        switch (statement) {
            case ExpressionStatement expressionStatement: {
                EvaluateExpression(expressionStatement.expression);
                if (expressionStatement.expression.GetResultType() != TupleType.EMPTY) {
                    PopUnused();
                }
                break;
            }
            case LetStatement letStatement: {
                PatternAddVariable(letStatement.pattern, letStatement.initializer);
                break;
            }
            case IfStatement ifStatement: {
                EvaluateExpression(ifStatement.condition);
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

                EvaluateExpression(whileStatement.condition);
                uint bfStart = byteCount;
                UndertaleInstruction bfInstruction = BranchFalse();

                breakPromises.Add(whileStatement, context);
                ExecuteBlockStatement(whileStatement.body);

                uint branchStart = byteCount;
                UndertaleInstruction branchInstruction = Branch();
                branchInstruction.JumpOffset = (int)(start - branchStart) / 4;

                uint endOffset = byteCount;
                bfInstruction.JumpOffset = (int)(endOffset - bfStart) / 4;
                foreach ((uint instructionStart, UndertaleInstruction instruction) in context.promises) {
                    instruction.JumpOffset = (int)(endOffset - instructionStart) / 4;
                }

                breakPromises.Remove(whileStatement);
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
        foreach (IStatement statement in blockStatement.statements) {
            ExecuteStatement(statement);
        }
    }

    void GenerateItem(IItem item) {
        switch (item) {
            case FunctionItem funItem: {
                UndertaleCode? code = funCodes.GetValueOrDefault(funItem.name);
                if (code != null) {
                    nowCode = code;
                    codeLocals = data!.CodeLocals.For(code);
                } else {
                    throw new NotImplementedException();
                }
                ExecuteBlockStatement(funItem.body);

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

    internal void Generate(UndertaleData data, FileNode[] nodes, Dictionary<string, UndertaleCode> funCodes) {
        this.data = data;
        foreach ((string name, UndertaleCode code) in funCodes) {
            this.funCodes.Add(name, code);
        }

        foreach (FileNode node in nodes) {
            foreach (IItem item in node.items) {
                GenerateItem(item);
            }
        }

        this.funCodes.Clear();
        this.data = null;
        stringIds.Clear();
        variables.Clear();
    }
}
