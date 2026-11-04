using System.Collections.Immutable;

namespace TypeScript.Intermediate.Type;

public class TupleType: IType {
    class ElementTypeComparer : IEqualityComparer<ImmutableArray<IType>> {
        bool IEqualityComparer<ImmutableArray<IType>>.Equals(ImmutableArray<IType> x, ImmutableArray<IType> y) {
            if (x.Length != y.Length) {
                return false;
            }

            for (int i = 0; i != x.Length; ++i) {
                if (x[i] != y[i]) {
                    return false;
                }
            }

            return true;
        }

        int IEqualityComparer<ImmutableArray<IType>>.GetHashCode(ImmutableArray<IType> obj) {
            if (obj.Length == 0) {
                return 0;
            }

            int result = obj[0].GetHashCode();
            for (int i = 1; i != obj.Length; ++i) {
                result ^= obj[i].GetHashCode();
            }

            return result;
        }
    }

    public readonly static TupleType EMPTY = new([]);
    readonly static Dictionary<ImmutableArray<IType>, TupleType> instantiated = new(new ElementTypeComparer()) {
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
