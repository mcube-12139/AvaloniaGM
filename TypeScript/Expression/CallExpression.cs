using TypeScript.Type;

namespace TypeScript.Expression;

public class CallExpression: IExpression {
    internal readonly ExpressionHead head;
    public readonly IExpression callee;
    public readonly IExpression[] parameters;

    FunctionType? calleeType;
    IType? type;

    public FunctionType GetCalleeType() => calleeType!;

    internal CallExpression(SourcePosition position, IExpression callee, IExpression[] parameters) {
        head = new(position);
        this.callee = callee;
        this.parameters = parameters;
    }

    ExpressionHead IExpression.GetHead() => head;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        calleeType = callee.SetCallSemantic(target, parameters, compiler);
        type = calleeType.resultType;
    }

    IType IExpression.GetResultType() => type!;
}

