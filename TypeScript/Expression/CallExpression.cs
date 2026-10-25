using TypeScript.Type;

namespace TypeScript.Expression;

public class CallExpression: IExpression {
    public readonly SourcePosition position;
    public readonly IExpression callee;
    public readonly IExpression[] parameters;

    FunctionType? calleeType;
    IType? type;

    public FunctionType GetCalleeType() => calleeType!;

    internal CallExpression(SourcePosition position, IExpression callee, IExpression[] parameters) {
        this.position = position;
        this.callee = callee;
        this.parameters = parameters;
    }

    SourcePosition IExpression.GetPosition() => position;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        callee.SetSemantic(null, compiler);
        calleeType = callee.GetResultType().AsFunctionType(position, compiler);
        calleeType.EnsureCanCall(parameters, position, compiler);

        type = calleeType.resultType;
    }

    IType IExpression.GetResultType() => type!;
}
