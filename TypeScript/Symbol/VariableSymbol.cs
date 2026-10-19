using TypeScript.Type;

namespace TypeScript.Symbol;

public class VariableSymbol(
    string name,
    IType type,
    bool initialized
) : IValueSymbol {
    public readonly string name = name;
    public readonly IType type = type;

    internal bool initialized = initialized;

    string ISymbol.GetName() => name;

    IType IValueSymbol.LazyGetValueType(TextPosition position, Compiler compiler) => type;

    IType IValueSymbol.GetValueType() => type;
}
