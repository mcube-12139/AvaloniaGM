namespace TypeScript.Type;

public class UnknownType : IType {
    string IType.GetAppearance() => "unknown";
}
