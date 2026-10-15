using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IType GetValueType(TextPosition position, Compiler compiler);
}
