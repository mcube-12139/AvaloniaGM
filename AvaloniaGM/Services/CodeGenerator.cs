using AvaloniaGM.Entities;
using AvaloniaGM.Entities.Symbol;
using AvaloniaGM.Exceptions;
using System.Collections.Generic;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace AvaloniaGM.Services {
    internal class CodeGenerator(UndertaleData data) {
        readonly Dictionary<string, ISymbol> symbols = new() {
            {"show_message", new FunctionSymbol(data.Functions.EnsureDefined("show_message", data.Strings), 1)}
        };
        readonly List<UndertaleInstruction> instructions = [];
        uint byteCount = 0;
        readonly Stack<UndertaleInstruction.DataType> types = [];
        readonly Dictionary<string, int> stringIds = new(data.Strings.Count);

        internal ISymbol GetSymbol(string name) {
            if (!symbols.TryGetValue(name, out ISymbol? symbol)) {
                throw new SemanticException();
            }

            return symbol;
        }

        internal void PushString(string value) {
            UndertaleString gameString;
            if (!stringIds.TryGetValue(value, out int id)) {
                gameString = new(value);
                id = data.Strings.Count;
                stringIds.Add(value, id);
                data.Strings.Add(gameString);
            } else {
                gameString = data.Strings[id];
            }

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
            replaced.LocalsCount = 1;

            instructions.Clear();
            byteCount = 0;
            types.Clear();
        }
    }
}
