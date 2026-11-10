namespace TypeScript.Intermediate.Type;

public class ArrayType: IType {
    public readonly IType elementType;

    internal ArrayType(IType elementType) {
        this.elementType = elementType;
    }
}
