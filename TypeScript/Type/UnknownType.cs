namespace TypeScript.Type;

public class UnknownType : IType {
    string IType.GetAppearance() => "unknown";

    bool IType.IsType(IType other) {
        return true;
    }
}
