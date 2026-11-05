namespace TypeScript.Intermediate.Type;

public class ArrayType: IType {
    readonly static Dictionary<IType, ArrayType> instantiated = [];

    public readonly IType elementType;

    ArrayType(IType elementType) {
        this.elementType = elementType;
    }

    internal static ArrayType Instantiate(IType elementType) {
        if (!instantiated.TryGetValue(elementType, out ArrayType? result)) {
            result = new(elementType);
            instantiated[elementType] = result;
        }

        return result;
    }
}
