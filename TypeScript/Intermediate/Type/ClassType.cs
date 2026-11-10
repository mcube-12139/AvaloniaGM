using System.Collections.Immutable;

namespace TypeScript.Intermediate.Type;

public class ClassType: IType {
    public readonly string name;
    public readonly ImmutableArray<Field> fields;

    internal ClassType(
        string name,
        ImmutableArray<Field> fields
    ) {
        this.name = name;
        this.fields = fields;
    }
}
