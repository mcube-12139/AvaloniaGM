namespace TypeScript.Type;

public class FunctionType(IType[] parameterTypes, IType resultType) : IType {
    public readonly IType[] parameterTypes = parameterTypes;
    public readonly IType resultType = resultType;

    public string GetAppearance() {
        return $"({string.Join(", ", parameterTypes.Select(type => type.GetAppearance()))}) => {resultType.GetAppearance()}";
    }

    bool IType.IsType(IType other) {
        if (other is not FunctionType otherFun) {
            return false;
        }

        if (!TypeHelp.IsType(resultType, otherFun.resultType)) {
            return false;
        }

        if (parameterTypes.Length != otherFun.parameterTypes.Length) {
            return false;
        }

        for (int i = 0; i != parameterTypes.Length; ++i) {
            if (!TypeHelp.IsType(parameterTypes[i], otherFun.parameterTypes[i])) {
                return false;
            }
        }

        return true;
    }

    FunctionType IType.AsFunctionType(TextPosition position, Compiler compiler) {
        return this;
    }
}
