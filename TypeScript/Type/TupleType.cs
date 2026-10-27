namespace TypeScript.Type;

public class TupleType(IType[] elementTypes): IType {
    public readonly static TupleType EMPTY = new([]);

    public readonly IType[] elementTypes = elementTypes;

    public string GetAppearance() {
        return $"({string.Join(", ", elementTypes.Select(type => type.GetAppearance()))}{(elementTypes.Length == 1 ? ", " : "")})";
    }
}
