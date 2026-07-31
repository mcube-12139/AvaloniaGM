using AvaloniaGM.Services;
using UndertaleModLib.Models;

namespace AvaloniaGM.Entities.Symbol {
    internal class FunctionSymbol(UndertaleFunction fun, int parameterCount) : IValueSymbol {
        IValueSymbol ISymbol.AsValue(TextPosition position, CodeGenerator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, CodeGenerator generator) {
            generator.Call(fun, parameterCount);
        }
    }
}
