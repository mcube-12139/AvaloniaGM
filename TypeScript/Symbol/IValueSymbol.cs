using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IValueSymbol ISymbol.AsValue(SourcePosition position) {
        return this;
    }
    IType GetValueType();
    internal IType LazyGetValueType(SourcePosition position, Compiler compiler);
}
