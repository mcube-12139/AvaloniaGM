namespace TypeScript.Type;

public class FunctionType(IType[] parameterTypes, IType resultType) : IType {
    public readonly IType[] parameterTypes = parameterTypes;
    public readonly IType resultType = resultType;

    public string GetAppearance() {
        return $"({string.Join(", ", parameterTypes.Select(type => type.GetAppearance()))}) => {resultType.GetAppearance()}";
    }

    bool IType.EqualType(IType other) {
        if (other is not FunctionType otherFun) {
            return false;
        }

        if (!resultType.EqualType(otherFun.resultType)) {
            return false;
        }

        if (parameterTypes.Length != otherFun.parameterTypes.Length) {
            return false;
        }

        for (int i = 0; i != parameterTypes.Length; ++i) {
            if (!parameterTypes[i].EqualType(otherFun.parameterTypes[i])) {
                return false;
            }
        }

        return true;
    }

    FunctionType IType.AsFunctionType(SourcePosition position, Compiler compiler) {
        return this;
    }
}
