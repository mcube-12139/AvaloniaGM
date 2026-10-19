using TypeScript.Type;

namespace TypeScript.Symbol;

public class ParameterSymbol(
    string name,
    IType type
) : IValueSymbol {
    public readonly string name = name;
    public readonly IType type = type;

    string ISymbol.GetName() => name;

    IValueSymbol ISymbol.AsValue(SourcePosition position, Compiler compiler) {
        return this;
    }

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => type;

    IType IValueSymbol.GetValueType() => type;
}
