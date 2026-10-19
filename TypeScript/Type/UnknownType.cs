namespace TypeScript.Type;

public class UnknownType : IType {
    string IType.GetAppearance() => "unknown";

    bool IType.EqualType(IType other) {
        return true;
    }
}
