using AvaloniaGM.Entities;
using AvaloniaGM.Entities.Symbols;
using AvaloniaGM.Exceptions;
using System.Collections.Generic;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.Services {
    internal class TypeScriptGenerator(UndertaleData data) {
        UndertaleCodeLocals codeLocals = null!;
        readonly Dictionary<string, ISymbol> symbols = new() {
            {"show_message", new FunctionSymbol(data.Functions.EnsureDefined("show_message", data.Strings), 1)}
        };
        // fuck Game Maker
        uint nextLocalId = 1;
        readonly List<UndertaleInstruction> instructions = [];
        uint byteCount = 0;
        readonly Stack<UndertaleInstruction.DataType> types = [];
        readonly Dictionary<string, int> stringIds = new(data.Strings.Count);

        internal void AddSymbol(string name, ISymbol symbol, TextPosition position) {
            if (!symbols.TryAdd(name, symbol)) {
                throw new SemanticException();
            }
        }

        internal ISymbol GetSymbol(string name) {
            if (!symbols.TryGetValue(name, out ISymbol? symbol)) {
                throw new SemanticException();
            }

            return symbol;
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
        }

        internal void PushType(UndertaleInstruction.DataType type) {
            types.Push(type);
        }

        internal void Generate(CodeRoot root, UndertaleCode replaced) {
            codeLocals = data.CodeLocals.For(replaced);
            if (stringIds.Count == 0) {
                for (int i = 0; i < data.Strings.Count; i++) {
                    stringIds[data.Strings[i].Content] = i;
                }
            }

            root.Generate(this);

            replaced.Replace(instructions);
            replaced.Length = byteCount;
            replaced.Offset = 0;
            replaced.ArgumentsCount = 0;
            replaced.LocalsCount = (uint)nextLocalId;

            instructions.Clear();
            byteCount = 0;
            types.Clear();
        }
    }
}
