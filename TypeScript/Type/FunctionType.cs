using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class FunctionType : IType {
    static readonly Dictionary<(IType[] parameterTypes, IType resultType), FunctionType> instantiated = [];

    public readonly IType[] parameterTypes;
    public readonly IType resultType;

    FunctionType(IType[] parameterTypes, IType resultType) {
        this.parameterTypes = parameterTypes;
        this.resultType = resultType;
    }

    internal static FunctionType Instantiate(IType[] parameterTypes, IType resultType) {
        if (!instantiated.TryGetValue((parameterTypes, resultType), out FunctionType? result)) {
            result = new(parameterTypes, resultType);
            instantiated.Add((parameterTypes, resultType), result);
        }

        return result;
    }

    internal FunctionType GetConcreteType() {
        return Instantiate([.. parameterTypes.Select(type => type.GetConcreteType())], resultType.GetConcreteType());
    }

    IType IType.GetConcreteType() => GetConcreteType();

    public string GetAppearance() {
        return $"({string.Join(", ", parameterTypes.Select(type => type.GetAppearance()))}) => {resultType.GetAppearance()}";
    }

    bool IType.IsType(IType other) => IsType(other);

    internal bool IsType(IType other) {
        if (other is not FunctionType otherFun) {
            return false;
        }

        if (!TypeHelp.IsType(resultType, otherFun.resultType)) {
            return false;
        }

        if (parameterTypes.Length != otherFun.parameterTypes.Length) {
            return false;
        }

        // 参数逆变
        for (int i = 0; i != parameterTypes.Length; ++i) {
            if (!TypeHelp.IsType(otherFun.parameterTypes[i], parameterTypes[i])) {
                return false;
            }
        }

        return true;
    }

    internal bool IsMethodType(IType other) {
        if (other is not FunctionType otherFun) {
            return false;
        }

        if (!TypeHelp.IsType(resultType, otherFun.resultType)) {
            return false;
        }

        if (parameterTypes.Length != otherFun.parameterTypes.Length) {
            return false;
        }

        // 参数逆变
        for (int i = 1; i < parameterTypes.Length; ++i) {
            if (!TypeHelp.IsType(otherFun.parameterTypes[i], parameterTypes[i])) {
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

    internal void EnsureCanCallMethod(IExpression[] parameters, SourcePosition position, Compiler compiler) {
        if (parameters.Length != parameterTypes.Length - 1) {
            throw new SemanticException(SemanticErrorType.WRONG_PARAMETER_COUNT, [parameters.Length.ToString(), (parameterTypes.Length - 1).ToString()], position);
        }
        for (int i = 0; i != parameters.Length; ++i) {
            IExpression parameter = parameters[i];
            IType definedType = parameterTypes[i + 1];
            parameter.SetSemantic(definedType, compiler);

            IType parameterType = parameter.GetResultType();
            if (!TypeHelp.IsType(parameterType, definedType)) {
                throw new SemanticException(SemanticErrorType.WRONG_TYPE, [parameterType.GetAppearance(), definedType.GetAppearance()], position);
            }
        }
    }

    internal GenericInferContext InferGenericParameters(IType? target, IExpression[] parameters, SourcePosition position, Compiler compiler) {
        if (parameters.Length != parameterTypes.Length) {
            throw new SemanticException(SemanticErrorType.WRONG_PARAMETER_COUNT, [parameters.Length.ToString(), parameterTypes.Length.ToString()], position);
        }
        GenericInferContext context = new(position, compiler);
        for (int i = 0; i != parameterTypes.Length; ++i) {
            parameterTypes[i].InferGenericByArgument(parameters[i], context);
        }
        resultType.InferGenericByTarget(target, context);

        return context;
    }
}
