using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public class CallExpression: IExpression {
    public readonly SourcePosition position;
    public readonly IExpression called;
    public readonly IExpression[] parameters;

    IType? type;

    internal CallExpression(SourcePosition position, IExpression called, IExpression[] parameters) {
        this.position = position;
        this.called = called;
        this.parameters = parameters;
    }

    SourcePosition IExpression.GetPosition() => position;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        called.SetSemantic(null, compiler);
        FunctionType funType = called.GetResultType().AsFunctionType(position, compiler);

        if (parameters.Length != funType.parameterTypes.Length) {
            throw new SemanticException(SemanticErrorType.WRONG_PARAMETER_COUNT, [parameters.Length.ToString(), funType.parameterTypes.Length.ToString()], position);
        }
        for (int i = 0; i != parameters.Length; ++i) {
            IExpression parameter = parameters[i];
            IType definedType = funType.parameterTypes[i];
            parameter.SetSemantic(definedType, compiler);

            IType parameterType = parameter.GetResultType();
            if (!TypeHelp.IsType(parameterType, definedType)) {
                throw new SemanticException(SemanticErrorType.WRONG_TYPE, [parameterType.GetAppearance(), definedType.GetAppearance()], position);
            }
        }

        type = funType.resultType;
    }

    IType IExpression.GetResultType() => type!;

    /*
    void IExpression.Call(Generator generator) {
        throw new System.NotImplementedException();
    }

    void IExpression.GetIndex(IExpression index, Generator generator) {
        throw new System.NotImplementedException();
    }

    void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
        throw new System.NotImplementedException();
    }
    */
}
