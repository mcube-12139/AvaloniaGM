using AvaloniaGM.TypeScript;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Symbols {
    internal class FunctionSymbol(UndertaleFunction fun, int parameterCount) : IValueSymbol {
        IValueSymbol ISymbol.AsValue(TextPosition position, Generator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, Generator generator) {
            generator.Call(fun, parameterCount);
        }

        void IValueSymbol.Load(TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
