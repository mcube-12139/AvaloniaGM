using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

internal class CallExpression(TextPosition position, IExpression called, IExpression[] parameters): IExpression {
    readonly TextPosition position = position;
    readonly IExpression called = called;
    readonly IExpression[] parameters = parameters;

    IType? type;

    TextPosition IExpression.GetPosition() => position;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        called.SetSemantic(null, compiler);
        FunctionType funType = called.GetResultType().AsFunctionType(position, compiler);

        if (parameters.Length != funType.parameterTypes.Length) {
            throw compiler.SemanticError(SemanticErrorType.WRONG_PARAMETER_COUNT, [parameters.Length.ToString(), funType.parameterTypes.Length.ToString()], position);
        }
        for (int i = 0; i != parameters.Length; ++i) {
            parameters[i].SetSemantic(funType.parameterTypes[i], compiler);
        }

        type = funType.resultType;
    }

    IType IExpression.GetResultType() {
        return type!;
    }

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
