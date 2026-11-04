using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate;

public class Parameter {
    public readonly string name;
    public readonly IType type;

    internal Parameter(string name, IType type) {
        this.name = name;
        this.type = type;
    }
}
