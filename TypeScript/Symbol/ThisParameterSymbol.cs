using TypeScript.Type;

namespace TypeScript.Symbol;

public class ThisParameterSymbol : IValueSymbol {
    readonly IType type;

    internal ThisParameterSymbol(IType type) {
        this.type = type;
    }

    string ISymbol.GetName() => "this";

    IType IValueSymbol.GetValueType() => type;

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => type;
}
