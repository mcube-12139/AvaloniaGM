
using System.Collections.Immutable;
using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate;

internal class TypeArrayComparer : IEqualityComparer<ImmutableArray<IType>> {
    internal static TypeArrayComparer COMPARER = new();

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