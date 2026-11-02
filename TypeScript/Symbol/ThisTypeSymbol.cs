using TypeScript.Type;

namespace TypeScript.Symbol;

/// <summary>
/// This 引用的符号，具体类型（implement 中）。
/// </summary>
public class ThisTypeSymbol : ITypeSymbol {
    public IType type;

    internal ThisTypeSymbol(IType type) {
        this.type = type;
    }

    string ISymbol.GetName() => "This";

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
