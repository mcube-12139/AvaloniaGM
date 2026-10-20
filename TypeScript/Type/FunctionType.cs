using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class FunctionType : IType {
    static readonly Dictionary<(IType[] parameterTypes, IType resultType), FunctionType> monomorphized = [];

    public readonly IType[] parameterTypes;
    public readonly IType resultType;

    FunctionType(IType[] parameterTypes, IType resultType) {
        this.parameterTypes = parameterTypes;
        this.resultType = resultType;
    }

    internal static FunctionType Monomorphize(IType[] parameterTypes, IType resultType) {
        if (!monomorphized.TryGetValue((parameterTypes, resultType), out FunctionType? result)) {
            result = new(parameterTypes, resultType);
            monomorphized.Add((parameterTypes, resultType), result);
        }

        return result;
    }

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

    internal void EnsureCanCall(IExpression[] parameters, SourcePosition position, Compiler compiler) {
        if (parameters.Length != parameterTypes.Length) {
            throw new SemanticException(SemanticErrorType.WRONG_PARAMETER_COUNT, [parameters.Length.ToString(), parameterTypes.Length.ToString()], position);
        }
        for (int i = 0; i != parameters.Length; ++i) {
            IExpression parameter = parameters[i];
            IType definedType = parameterTypes[i];
            parameter.SetSemantic(definedType, compiler);

            IType parameterType = parameter.GetResultType();
            if (!TypeHelp.IsType(parameterType, definedType)) {
                throw new SemanticException(SemanticErrorType.WRONG_TYPE, [parameterType.GetAppearance(), definedType.GetAppearance()], position);
            }
        }
    }
}
