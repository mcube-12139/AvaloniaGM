using System.Collections.Immutable;

namespace TypeScript.Intermediate.Type;

public class TupleType: IType {
    public readonly static TupleType EMPTY = new([]);
    readonly static Dictionary<ImmutableArray<IType>, TupleType> instantiated = new(TypeArrayComparer.COMPARER) {
        { [], EMPTY  },
    };

    readonly ImmutableArray<IType> elementTypes;

    TupleType(ImmutableArray<IType> elementTypes) {
        this.elementTypes = elementTypes;
    }

    internal static TupleType Instantiate(ImmutableArray<IType> elementTypes) {
        if (!instantiated.TryGetValue(elementTypes, out TupleType? result)) {
            result = new(elementTypes);
            instantiated[elementTypes] = result;
        }

        return result;
    }
}
