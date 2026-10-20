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
        funType.EnsureCanCall(parameters, position, compiler);

        type = funType.resultType;
    }

    IType IExpression.GetResultType() => type!;
}
