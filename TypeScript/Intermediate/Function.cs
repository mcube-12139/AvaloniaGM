using System.Collections.Immutable;

namespace TypeScript.Intermediate;

public class Function {
    public readonly string name;
    public readonly ImmutableArray<Parameter> parameters;
    public readonly Type.IType resultType;
    public readonly ImmutableArray<Type.IType> localTypes;
    public readonly IFunctionBody body;

    internal Function(
        string name,
        ImmutableArray<Parameter> parameters,
        Type.IType resultType,
        ImmutableArray<Type.IType> localTypes,
        IFunctionBody body
    ) {
        this.name = name;
        this.parameters = parameters;
        this.resultType = resultType;
        this.localTypes = localTypes;
        this.body = body;
    }
}
