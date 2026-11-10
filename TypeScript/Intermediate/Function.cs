using System.Collections.Immutable;

namespace TypeScript.Intermediate;

public class Function {
    public readonly string name;
    public readonly ImmutableArray<Parameter> parameters;
    public readonly Type.IType resultType;
    public readonly IFunctionBody body;

    internal Function(
        string name,
        ImmutableArray<Parameter> parameters,
        Type.IType resultType,
        IFunctionBody body
    ) {
        this.name = name;
        this.parameters = parameters;
        this.resultType = resultType;
        this.body = body;
    }
}
