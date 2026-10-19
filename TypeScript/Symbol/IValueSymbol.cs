using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IValueSymbol ISymbol.AsValue(TextPosition position, Compiler compiler) {
        return this;
    }
    IType GetValueType();
    internal IType LazyGetValueType(TextPosition position, Compiler compiler);
}
