using System.Collections.Immutable;

namespace TypeScript.Intermediate.Type;

public class TupleType: IType {
    public readonly ImmutableArray<IType> elementTypes;

    internal TupleType(ImmutableArray<IType> elementTypes) {
        this.elementTypes = elementTypes;
    }
}
