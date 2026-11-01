using TypeScript.Type;

namespace TypeScript.Symbol;

public class ThisTypeSymbol : ITypeSymbol {
    public readonly IType type;

    internal ThisTypeSymbol(IType type) {
        this.type = type;
    }

    string ISymbol.GetName() => "This";

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
