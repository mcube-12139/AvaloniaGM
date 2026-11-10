using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate;

public class Field {
    public readonly string name;
    public readonly IType type;

    internal Field(string name, IType type) {
        this.name = name;
        this.type = type;
    }
}
