using TypeScript.GenerateContext;
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
        this.head = new(position);
        this.callee = callee;
        this.parameters = parameters;
    }

    ExpressionHead IExpression.GetHead() => head;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        callee.SetSemantic(null, compiler);
        calleeType = callee.GetResultType().AsFunctionType(head.position, compiler);
        calleeType.EnsureCanCall(parameters, head.position, compiler);

        type = calleeType.resultType;
    }

    IType IExpression.GetResultType() => type!;

    internal void GenerateParameters(StatementContext context) {
        foreach (IExpression parameter in parameters) {
            ExpressionHelp.Generate(parameter, context);
        }
    }

    void IExpression.Generate(StatementContext context) {
        callee.GenerateCall(this, context);
    }
}

