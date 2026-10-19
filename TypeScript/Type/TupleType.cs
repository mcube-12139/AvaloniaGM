namespace TypeScript.Type;

public class TupleType(IType[] elementTypes): IType {
    public readonly static TupleType EMPTY = new([]);

    public readonly IType[] elementTypes = elementTypes;

    public string GetAppearance() {
        return $"({string.Join(", ", elementTypes.Select(type => type.GetAppearance()))}{(elementTypes.Length == 1 ? ", " : "")})";
    }

    bool IType.EqualType(IType other) {
        if (other is not TupleType otherTuple) {
            return false;
        }

        if (elementTypes.Length != otherTuple.elementTypes.Length) {
            return false;
        }

        for (int i = 0; i != elementTypes.Length; ++i) {
            if (!elementTypes[i].EqualType(otherTuple.elementTypes[i])) {
                return false;
            }
        }

        return true;
    }
}
