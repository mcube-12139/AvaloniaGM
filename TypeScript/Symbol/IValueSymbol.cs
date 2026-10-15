using TypeScript.Type;

namespace TypeScript.Symbol;

internal interface IValueSymbol: ISymbol {
    IType GetValueType(TextPosition position, Compiler compiler);
}
