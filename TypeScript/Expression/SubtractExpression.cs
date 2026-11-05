using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Expression;

internal class SubtractExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    internal readonly IExpression left = left;
    internal readonly IExpression right = right;

    IType? type;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        type = left.GetResultType().GetSubtractResultType(right, position, compiler);
    }

    void IExpression.Generate(StatementContext context) {
        ExpressionHelp.Generate(left, context);
        ExpressionHelp.Generate(right, context);
        left.GetResultType().GenerateSubtract(position, context);
    }
}
