namespace TypeScript.Type;

internal class TypeHelp {
    internal static bool IsType(IType a, IType b) {
        if (b == PrimitiveType.UNKNOWN) {
            return true;
        }

        return a.EqualType(b);
    }
}
