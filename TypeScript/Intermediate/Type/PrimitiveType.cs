namespace TypeScript.Intermediate.Type;

public class PrimitiveType: IType {
    public readonly static PrimitiveType INTEGER = new();
    public readonly static PrimitiveType STRING = new();
}
