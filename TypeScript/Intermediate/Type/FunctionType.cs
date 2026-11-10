using System.Collections.Immutable;

namespace TypeScript.Intermediate.Type;

public class FunctionType: IType {
    public readonly ImmutableArray<IType> parameterTypes;
    public readonly IType resultType;

    internal FunctionType(ImmutableArray<IType> parameterTypes, IType resultType) {
        this.parameterTypes = parameterTypes;
        this.resultType = resultType;
    }
}
