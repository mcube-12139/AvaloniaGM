using System.Collections.Immutable;

namespace TypeScript.Intermediate;

public class VirtualFunction {
    public readonly string name;
    public readonly ImmutableArray<Parameter> parameters;
    public readonly Type.IType resultType;

    internal VirtualFunction(
        string name,
        ImmutableArray<Parameter> parameters,
        Type.IType resultType
    ) {
        this.name = name;
        this.parameters = parameters;
        this.resultType = resultType;
    }
}
