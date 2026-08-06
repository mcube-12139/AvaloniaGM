using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Symbols {
    internal class PrimitiveTypeSymbol(string name, IType type) : ITypeSymbol {
        ITypeSymbol ISymbol.AsType(TextPosition position, Generator generator) =>
            this;

        IValueSymbol ISymbol.AsValue(TextPosition position, Generator generator) =>
            throw generator.SemanticError(SemanticErrorType.NOT_VALUE, [name], position);

        IType ITypeSymbol.GetSharkType(TextPosition position, Generator generator) =>
            type;
    }
}
