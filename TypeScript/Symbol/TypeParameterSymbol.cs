using TypeScript.Field;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class TypeParameterSymbol : ITypeSymbol {
    public readonly string name;
    public readonly BoundedType type;

    internal TypeParameterSymbol(string name, BoundedType type) {
        this.name = name;
        this.type = type;
    }

    string ISymbol.GetName() => name;

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
