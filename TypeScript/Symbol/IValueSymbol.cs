using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IType GetValueType();
    internal IType LazyGetValueType(TextPosition position, Compiler compiler);
}
