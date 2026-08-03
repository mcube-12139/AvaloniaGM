using AvaloniaGM.Services;
using UndertaleModLib.Models;

namespace AvaloniaGM.Entities.Symbols {
    internal class FunctionSymbol(UndertaleFunction fun, int parameterCount) : IValueSymbol {
        IValueSymbol ISymbol.AsValue(TextPosition position, TypeScriptGenerator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, TypeScriptGenerator generator) {
            generator.Call(fun, parameterCount);
        }

        void IValueSymbol.Load(TextPosition position, TypeScriptGenerator generator) {
            throw new System.NotImplementedException();
        }
    }
}
