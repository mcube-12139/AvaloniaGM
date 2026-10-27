using TypeScript.Symbol;
using TypeScript.TypeNode;

namespace TypeScript.GenericParameter;

public class TypeParameter: IGenericParameter {
    public readonly SourcePosition position;
    public readonly string name;
    public readonly Path[]? bounds;
    public readonly ITypeNode? defaultType;

    TypeParameterSymbol? symbol;

    internal TypeParameter(SourcePosition position, string name, Path[]? bounds, ITypeNode? defaultType) {
        this.position = position;
        this.name = name;
        this.bounds = bounds;
        this.defaultType = defaultType;
    }

    void IGenericParameter.AddSymbol(Compiler compiler) {
        IConcept[] typeBounds;
        if (bounds != null) {
            typeBounds = [.. bounds.Select(bound => bound.GetConcept(compiler))];
        } else {
            typeBounds = [];
        }
        symbol = new(name, new(typeBounds));
        compiler.AddSymbol(name, symbol, position);
    }
}
