using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Symbols;
using System.Collections.Generic;
using System.Xml.Linq;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript {
    internal class Generator(UndertaleData data) {
        string source = string.Empty;
        UndertaleCodeLocals codeLocals = null!;
        Dictionary<string, ISymbol> symbols = new() {
            {"show_message", new FunctionSymbol(data.Functions.EnsureDefined("show_message", data.Strings), 1)}
        };
        readonly List<Dictionary<string, ISymbol>> blocks = [];
        // fuck Game Maker
        uint nextLocalId = 1;
        readonly List<UndertaleInstruction> instructions = [];
        uint byteCount = 0;
        readonly Stack<UndertaleInstruction.DataType> types = [];
        readonly Dictionary<string, int> stringIds = new(data.Strings.Count);

        internal SemanticException SemanticError(SemanticErrorType type, string[] parameters, TextPosition position) {
            return new SemanticException(type, parameters, source, position);
        }

        internal void AddSymbol(string name, ISymbol symbol, TextPosition position) {
            if (!symbols.TryAdd(name, symbol)) {
                throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], source, position);
            }
        }

        internal ISymbol GetSymbol(string name, TextPosition position) {
            for (int i = blocks.Count - 1; i != -1; --i) {
                if (blocks[i].TryGetValue(name, out ISymbol? symbol)) {
                    return symbol;
                }
            }

            throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [name], source, position);
        }

        internal void EnterBlock() {
            symbols = [];
            blocks.Add(symbols);
        }

        internal void LeaveBlock() {
            symbols = blocks[^1];
            blocks.RemoveAt(blocks.Count - 1);
        }

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

        internal void PushString(string value) {
            (UndertaleString gameString, int id) = GetString(value);

            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Type1 = UndertaleInstruction.DataType.String,
                ValueString = new UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>(gameString, id)
            });
            byteCount += 8;
        }

        internal void PushInt16(short value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.PushI,
                Type1 = UndertaleInstruction.DataType.Int16,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueShort = value
            });
            byteCount += 4;
        }

        internal void PushInt32(int value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Type1 = UndertaleInstruction.DataType.Int32,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueInt = value
            });
            byteCount += 8;
        }

        internal void PushInt64(long value) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Push,
                Type1 = UndertaleInstruction.DataType.Int64,
                Type2 = UndertaleInstruction.DataType.Double,
                ValueLong = value
            });
            byteCount += 12;
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
        }

        internal void PopUnused() {
            UndertaleInstruction.DataType type = types.Pop();
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Popz,
                Type1 = type
            });
            byteCount += 4;
        }

        internal void Pop(UndertaleVariable variable, UndertaleInstruction.DataType dataType, UndertaleInstruction.VariableType variableType) {
            UndertaleInstruction.DataType valueType = types.Pop();
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Pop,
                Type1 = dataType,
                Type2 = valueType,
                ValueVariable = variable,
                TypeInst = variable.InstanceType,
                ReferenceType = variableType
            });
            byteCount += 8;
        }

        internal void PushLocal(UndertaleVariable variable, UndertaleInstruction.DataType dataType, UndertaleInstruction.VariableType variableType) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.PushLoc,
                Type1 = dataType,
                ValueVariable = variable,
                ReferenceType = variableType,
                TypeInst = variable.InstanceType
            });
            byteCount += 8;
        }

        internal void Call(UndertaleFunction fun, int parameterCount) {
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

            PushType(UndertaleInstruction.DataType.Variable);
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

            PushType(UndertaleInstruction.DataType.Variable);
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

            PushType(UndertaleInstruction.DataType.Variable);
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

            PushType(UndertaleInstruction.DataType.Variable);
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

            blocks.Add(symbols);

            root.Generate(this);

            replaced.Replace(instructions);
            replaced.Length = byteCount;
            replaced.Offset = 0;
            replaced.ArgumentsCount = 0;
            replaced.LocalsCount = (uint)nextLocalId;

            blocks.Clear();
            instructions.Clear();
            byteCount = 0;
            types.Clear();
        }
    }
}
