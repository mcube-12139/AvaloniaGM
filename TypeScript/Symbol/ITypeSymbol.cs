using TypeScript.Type;

namespace TypeScript.Symbol;

public interface ITypeSymbol: ISymbol {
    IType GetSharkType();
    ITypeSymbol ISymbol.AsType(TextPosition position, Compiler compiler) {
        return this;
    }
}
