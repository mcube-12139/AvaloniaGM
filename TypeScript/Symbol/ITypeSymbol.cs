using TypeScript.Type;

namespace TypeScript.Symbol;

public interface ITypeSymbol: ISymbol {
    IType GetSelfType();
    ITypeSymbol ISymbol.AsType(SourcePosition position, Compiler compiler) {
        return this;
    }
}
