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
        readonly Stack<UndertaleInstruction.DataType> types = [];
        readonly Dictionary<string, int> stringIds = [];

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
        }

        internal void Convert(UndertaleInstruction.DataType target) {
            UndertaleInstruction.DataType source = types.Pop();

            if (source != target) {
                instructions.Add(new() {
                    Kind = UndertaleInstruction.Opcode.Conv,
                    Type1 = source,
                    Type2 = target
                });
            }
        }

        internal void PopUnused() {
            UndertaleInstruction.DataType type = types.Pop();
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Popz,
                Type1 = type
            });
        }

        internal void Call(UndertaleFunction fun, int parameterCount) {
            instructions.Add(new() {
                Kind = UndertaleInstruction.Opcode.Call,
                Type1 = UndertaleInstruction.DataType.Int32,
                ArgumentsCount = (ushort)parameterCount,
                ValueFunction = fun
            });
        }

        internal void PushType(UndertaleInstruction.DataType type) {
            types.Push(type);
        }

        internal void Generate(CodeRoot root) {
            root.Generate(this);
        }
    }
}
