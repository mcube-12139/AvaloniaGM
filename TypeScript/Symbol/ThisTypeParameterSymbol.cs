using TypeScript.Type;

namespace TypeScript.Symbol;

/// <summary>
/// This 引用的符号，类型参数（concept, interface 中）。
/// </summary>
public class ThisTypeParameterSymbol : ITypeSymbol {
    public GenericType type;

    internal ThisTypeParameterSymbol(GenericType type) {
        this.type = type;
    }

    string ISymbol.GetName() => "This";

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
