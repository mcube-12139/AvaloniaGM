using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Statements;
using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.Types;
using System.Collections.Generic;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript {
    internal class Generator(UndertaleData data) {
        string source = string.Empty;

        readonly Stack<FunctionContext> functionContexts = [];
        IType resultType = TupleType.EMPTY;
        UndertaleCodeLocals codeLocals = null!;
        List<UndertaleInstruction> instructions = [];
        uint byteCount = 0;
        // fuck Game Maker
        uint nextLocalId = 1;
        uint nextParameterIndex = 0;

        NameSpace space = new();
        readonly List<NameSpace> spaces = [];
        readonly List<ILoopStatement> loops = [];
        uint nextFunctionId = 0;
        readonly Stack<UndertaleInstruction.DataType> types = [];
        readonly Dictionary<string, int> stringIds = new(data.Strings.Count);
        readonly Dictionary<string, UndertaleVariable> selfVariables = [];

        internal SemanticException SemanticError(SemanticErrorType type, string[] parameters, TextPosition position) {
            return new SemanticException(type, parameters, source, position);
        }

        internal void AddSymbol(string name, ISymbol symbol, TextPosition position) {
            if (!space.TryAddSymbol(name, symbol)) {
                throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], source, position);
            }
        }

        internal ISymbol GetSymbol(string name, TextPosition position) {
            for (int i = spaces.Count - 1; i != -1; --i) {
                ISymbol? symbol = spaces[i].GetSymbol(name);
                if (symbol != null) {
                    return symbol;
                }
            }

            throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [name], source, position);
        }

        internal ILoopStatement GetLoop(string? label, TextPosition position) {
            if (label == null) {
                if (loops.Count != 0) {
                    return loops[^1];
                } else {
                    throw new SemanticException(SemanticErrorType.NOT_IN_LOOP, [], source, position);
                }
            } else {
                for (int i = spaces.Count - 1; i != -1; --i) {
                    ILoopStatement? statement = spaces[i].GetLoop(label);
                    if (statement != null) {
                        return statement;
                    }
                }

                throw new SemanticException(SemanticErrorType.LABEL_NOT_EXIST, [label], source, position);
            }
        }

        internal void EnterNameSpace() {
            space = new();
            spaces.Add(space);
        }

        internal void LeaveNameSpace() {
            spaces.RemoveAt(spaces.Count - 1);
            space = spaces[^1];
        }

        internal void EnterLoop(string? label, ILoopStatement statement) {
            loops.Add(statement);
            if (label != null) {
                space.TryAddLoop(label, statement);
            }
        }

        internal void LeaveLoop() {
            loops.RemoveAt(loops.Count - 1);
        }

        internal void EnterFunction(UndertaleCode code, IType resultType) {
            functionContexts.Push(new(instructions, byteCount, resultType, codeLocals, nextLocalId));

            instructions = [];
            byteCount = 0;
            this.resultType = resultType;
            codeLocals = data.CodeLocals.For(code);
            nextLocalId = 1;

            nextParameterIndex = 0;

            EnterNameSpace();
        }

        internal void LeaveFunction(UndertaleCode code) {
            code.Replace(instructions);
            code.Length = byteCount;
            code.Offset = 0;
            code.ArgumentsCount = 0;
            code.LocalsCount = nextLocalId;

            FunctionContext context = functionContexts.Pop();

            instructions = context.instructions;
            byteCount = context.byteCount;
            resultType = context.resultType;
            codeLocals = context.codeLocals;
            nextLocalId = context.nextLocalId;

            LeaveNameSpace();
        }

        internal uint NextParameterIndex() {
            uint result = nextParameterIndex;
            ++nextParameterIndex;
            return result;
        }

        internal IType GetResultType() => resultType;

        internal (UndertaleString, int id) GetString(string value) {
            UndertaleString gameString;

            if (!stringIds.TryGetValue(value, out int id)) {
                gameString = new(value);
                id = data.Strings.Count;
                stringIds.Add(value, id);
                data.Strings.Add(gameString);
            } else {
                gameString = data.Strings[id];
            }

            return (gameString, id);
        }

        internal UndertaleVariable AddLocalVariable(string name) {
            (UndertaleString gameString, int id) = GetString(name);
            uint variableId = nextLocalId;
            ++nextLocalId;

            UndertaleVariable result = data.Variables.DefineLocal(data, (int)variableId, gameString, id);
            codeLocals.Locals.Add(new() {
                Index = (uint)id,
                Name = gameString
            });

            if (nextLocalId > data.MaxLocalVarCount) {
                data.MaxLocalVarCount = nextLocalId;
            }

            return result;
        }

        internal UndertaleVariable AddSelfVariable(string name, bool builtin) {
            (UndertaleString gameString, int id) = GetString(name);

            if (!selfVariables.TryGetValue(name, out UndertaleVariable? result)) {
                result = data.Variables.Define(gameString, id, UndertaleInstruction.InstanceType.Self, builtin, data);
            }

            return result;
        }

        internal (UndertaleFunction, UndertaleCode) CreateFunction(string name) {
            string funName = $"{name}_{nextFunctionId}";
            ++nextFunctionId;

            var entry = UndertaleCode.CreateEmptyEntry(data, $"gml_Script_{funName}");
            UndertaleString nameStr = data.Strings.MakeString(funName);
            UndertaleScript script = new() {
                Name = nameStr,
                Code = entry
            };

            data.Scripts.Add(script);

            UndertaleFunction fun = data.Functions.EnsureDefined(funName, data.Strings);
            return (fun, entry);
        }

        internal void PushString(string value) {
            (UndertaleString gameString, int id) = GetString(value);

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Type1 = UndertaleInstruction.DataType.String,
                ValueString = new UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>(gameString, id)
            });
            byteCount += 8;

            PushType(UndertaleInstruction.DataType.String);
        }

        internal void PushBoolean(bool value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.PushI,
                Type1 = UndertaleInstruction.DataType.Int16,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueShort = value ? (short)1 : (short)0
            });
            byteCount += 4;

            PushType(UndertaleInstruction.DataType.Boolean);
        }

        internal void PushInt16(short value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.PushI,
                Type1 = UndertaleInstruction.DataType.Int16,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueShort = value
            });
            byteCount += 4;

            PushType(UndertaleInstruction.DataType.Int32);
        }

        internal void PushInt32(int value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Type1 = UndertaleInstruction.DataType.Int32,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueInt = value
            });
            byteCount += 8;

            PushType(UndertaleInstruction.DataType.Int32);
        }

        internal void PushInt64(long value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Type1 = UndertaleInstruction.DataType.Int64,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueLong = value
            });
            byteCount += 12;

            PushType(UndertaleInstruction.DataType.Int64);
        }

        internal void Convert(UndertaleInstruction.DataType target) {
            UndertaleInstruction.DataType source = types.Pop();

            if (source != target) {
                instructions.Add(new() {
                    Kind = UndertaleInstruction.Opcode.Conv,
                    Type1 = source,
                    Type2 = target
                });
                byteCount += 4;
            }

            PushType(target);
        }

        internal void PopUnused() {
            UndertaleInstruction.DataType type = types.Pop();
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Popz,
                Type1 = type
            });
            byteCount += 4;
        }

        internal void Store(UndertaleVariable variable, UndertaleInstruction.VariableType variableType) {
            UndertaleInstruction.DataType valueType = types.Pop();
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Pop,
                Type1 = UndertaleInstruction.DataType.Variable,
                Type2 = valueType,
                ValueVariable = variable,
                TypeInst = variable.InstanceType,
                ReferenceType = variableType
            });
            byteCount += 8;

            if (variableType == UndertaleInstruction.VariableType.Array) {
                PopType();
                PopType();
            }
        }

        internal void Load(UndertaleVariable variable, UndertaleInstruction.VariableType variableType) {
            UndertaleInstruction.Opcode opcode;

            if (variable.InstanceType == UndertaleInstruction.InstanceType.Local) {
                opcode = UndertaleInstruction.Opcode.PushLoc;
            } else if (variable.InstanceType == UndertaleInstruction.InstanceType.Self) {
                if (variable.VarID == (int)UndertaleInstruction.InstanceType.Builtin) {
                    opcode = UndertaleInstruction.Opcode.PushBltn;
                } else {
                    opcode = UndertaleInstruction.Opcode.Push;
                }
            } else {
                throw new System.NotImplementedException();
            }

            instructions.Add(new() {
                Kind = opcode,
                Type1 = UndertaleInstruction.DataType.Variable,
                ValueVariable = variable,
                ReferenceType = variableType,
                TypeInst = variable.InstanceType
            });
            byteCount += 8;

            if (variableType == UndertaleInstruction.VariableType.Array) {
                PopType();
                PopType();
            }

            PushType(UndertaleInstruction.DataType.Variable);
        }

        internal void Call(UndertaleFunction fun, int parameterCount) {
            for (int i = parameterCount; i != 0; --i) {
                PopType();
            }

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Call,
                Type1 = UndertaleInstruction.DataType.Int32,
                ArgumentsCount = (ushort)parameterCount,
                ValueFunction = fun
            });
            byteCount += 8;

            PushType(UndertaleInstruction.DataType.Variable);
        }

        internal void Add() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Add,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else if (leftType == UndertaleInstruction.DataType.String && rightType == UndertaleInstruction.DataType.String) {
                PushType(UndertaleInstruction.DataType.String);
            } else if (leftType == UndertaleInstruction.DataType.Double && rightType == UndertaleInstruction.DataType.Double) {
                PushType(UndertaleInstruction.DataType.Double);
            } else {
                PushType(UndertaleInstruction.DataType.Variable);
            }
        }

        internal void Subtract() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Sub,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else if (leftType == UndertaleInstruction.DataType.Double && rightType == UndertaleInstruction.DataType.Double) {
                PushType(UndertaleInstruction.DataType.Double);
            } else {
                PushType(UndertaleInstruction.DataType.Variable);
            }
        }

        internal void Multiply() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Mul,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else if (leftType == UndertaleInstruction.DataType.Double && rightType == UndertaleInstruction.DataType.Double) {
                PushType(UndertaleInstruction.DataType.Double);
            } else {
                PushType(UndertaleInstruction.DataType.Variable);
            }
        }

        internal void Divide() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Div,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else if (leftType == UndertaleInstruction.DataType.Double && rightType == UndertaleInstruction.DataType.Double) {
                PushType(UndertaleInstruction.DataType.Double);
            } else {
                PushType(UndertaleInstruction.DataType.Variable);
            }
        }

        internal void Modulo() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Mod,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else {
                PushType(UndertaleInstruction.DataType.Variable);
            }
        }

        internal void LeftShift() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Shl,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            PushType(UndertaleInstruction.DataType.Int64);
        }

        internal void RightShift() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Shr,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            PushType(UndertaleInstruction.DataType.Int64);
        }

        internal void BitAnd() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.And,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else {
                throw new System.Exception("wtf");
            }
        }

        internal void BitOr() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Or,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else {
                throw new System.Exception("wtf");
            }
        }

        internal void BitXor() {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Xor,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            if (leftType == UndertaleInstruction.DataType.Int32 && rightType == UndertaleInstruction.DataType.Int32) {
                PushType(UndertaleInstruction.DataType.Int32);
            } else if (leftType == UndertaleInstruction.DataType.Int64 && rightType == UndertaleInstruction.DataType.Int64) {
                PushType(UndertaleInstruction.DataType.Int64);
            } else {
                throw new System.Exception("wtf");
            }
        }

        internal void Compare(UndertaleInstruction.ComparisonType comparisonType) {
            UndertaleInstruction.DataType rightType = types.Pop();
            UndertaleInstruction.DataType leftType = types.Pop();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Cmp,
                ComparisonKind = comparisonType,
                Type1 = rightType,
                Type2 = leftType
            });
            byteCount += 4;

            PushType(UndertaleInstruction.DataType.Boolean);
        }

        internal UndertaleInstruction Branch() {
            UndertaleInstruction result = new() {
                Kind = UndertaleInstruction.Opcode.B
            };

            instructions.Add(result);
            byteCount += 4;

            return result;
        }

        internal UndertaleInstruction BranchTrue() {
            UndertaleInstruction result = new() {
                Kind = UndertaleInstruction.Opcode.Bt
            };

            instructions.Add(result);
            byteCount += 4;

            PopType();

            return result;
        }

        internal UndertaleInstruction BranchFalse() {
            UndertaleInstruction result = new() {
                Kind = UndertaleInstruction.Opcode.Bf
            };

            instructions.Add(result);
            byteCount += 4;

            PopType();

            return result;
        }

        internal void Return() {
            Convert(UndertaleInstruction.DataType.Variable);
            PopType();

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Ret,
                Type1 = UndertaleInstruction.DataType.Variable
            });
            byteCount += 4;
        }

        internal void Exit() {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Exit,
                Type1 = UndertaleInstruction.DataType.Int32
            });
            byteCount += 4;
        }

        internal uint GetByteCount() {
            return byteCount;
        }

        internal void PushType(UndertaleInstruction.DataType type) {
            types.Push(type);
        }

        internal void PopType() {
            types.Pop();
        }

        internal UndertaleInstruction.DataType PeekType() {
            return types.Peek();
        }

        internal void Generate(string source, CodeRoot root, UndertaleCode replaced) {
            this.source = source;

            codeLocals = data.CodeLocals.For(replaced);
            if (stringIds.Count == 0) {
                for (int i = 0; i < data.Strings.Count; i++) {
                    stringIds[data.Strings[i].Content] = i;
                }
            }

            space.TryAddSymbol("show_message", FunctionSymbol.NewResolved("show_message", data.Functions.EnsureDefined("show_message", data.Strings), new FunctionType([PrimitiveType.INTEGER], PrimitiveType.DOUBLE)));
            space.TryAddSymbol("int", new PrimitiveTypeSymbol("int", PrimitiveType.INTEGER));
            spaces.Add(space);

            root.Generate(this);

            replaced.Replace(instructions);
            replaced.Length = byteCount;
            replaced.Offset = 0;
            replaced.ArgumentsCount = 0;
            replaced.LocalsCount = nextLocalId;

            space.Clear();
            spaces.Clear();
            instructions.Clear();
            byteCount = 0;
            types.Clear();
        }
    }
}
