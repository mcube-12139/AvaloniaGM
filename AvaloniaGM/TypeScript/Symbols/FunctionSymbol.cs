using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Items;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Symbols {
    internal class FunctionSymbol(string name, UndertaleFunction fun, FunctionType? type, FunctionItem? item) : IValueSymbol {
        FunctionType? type = type;
        FunctionItem? item = item;

        internal static FunctionSymbol NewResolved(string name, UndertaleFunction fun, FunctionType type) =>
            new(name, fun, type, null);

        internal static FunctionSymbol NewUnresolved(string name, UndertaleFunction fun, FunctionItem item) =>
            new(name, fun, null, item);

        void Resolve(Generator generator) {
            type ??= item!.ResolveType(generator);
        }

        IValueSymbol ISymbol.AsValue(TextPosition position, Generator generator) {
            return this;
        }

        ITypeSymbol ISymbol.AsType(TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_TYPE, [name], position);
        }

        void IValueSymbol.Call(TextPosition position, Generator generator) {
            Resolve(generator);
            type!.Call(fun, generator);
        }

        IType IValueSymbol.GetValueType(TextPosition position, Generator generator) {
            Resolve(generator);
            return type!;
        }

        void IValueSymbol.Load(TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IValueSymbol.Store(TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_WRITEABLE, [name], position);
        }
    }
}
