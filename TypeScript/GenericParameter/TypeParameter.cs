using TypeScript.Symbol;
using TypeScript.Type;
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

    IGenericParameterSymbol IGenericParameter.AddSymbol(Compiler compiler) {
        IConceptSymbol[] typeBounds;
        if (bounds != null) {
            foreach (Path bound in bounds) {
                ((ITypeNode)bound).SetSemantic(compiler);
            }
            typeBounds = [.. bounds.Select(bound => bound.GetSymbol().AsConcept(position))];
        } else {
            typeBounds = [];
        }
        symbol = new(name, GenericType.WithBounds(typeBounds, position, compiler));
        compiler.AddSymbol(name, symbol, position);

        return symbol;
    }
}
