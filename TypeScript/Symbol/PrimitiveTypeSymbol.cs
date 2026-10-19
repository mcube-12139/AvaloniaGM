using TypeScript.Type;

namespace TypeScript.Symbol;

public class PrimitiveTypeSymbol(string name, IType type) : ITypeSymbol {
    public readonly string name = name;
    public readonly IType type = type;

    string ISymbol.GetName() => name;

    IType ITypeSymbol.GetSelfType() => type;
    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
