using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Symbols {
    internal class FunctionSymbol(UndertaleFunction fun, FunctionType type) : IValueSymbol {
        IValueSymbol ISymbol.AsValue(TextPosition position, Generator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, Generator generator) {
            type.Call(fun, generator);
        }

        IType IValueSymbol.GetValueType(TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IValueSymbol.Load(TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IValueSymbol.Store(TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_WRITEABLE, [fun.Name.Content], position);
        }
    }
}
