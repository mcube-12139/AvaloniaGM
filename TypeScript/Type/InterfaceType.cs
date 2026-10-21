namespace TypeScript.Type;

public class InterfaceType : IType {
    public readonly string name;

    internal readonly NameSpace space;

    internal InterfaceType(string name) {
        this.name = name;

        space = new();
    }

    bool IType.EqualType(IType other) {
        return other == this;
    }

    string IType.GetAppearance() => name;
}
