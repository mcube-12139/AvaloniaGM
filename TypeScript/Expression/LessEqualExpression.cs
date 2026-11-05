using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Expression;

internal class LessEqualExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    internal readonly IExpression left = left;
    internal readonly IExpression right = right;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanLessEqual(right, position, compiler);
    }

    void IExpression.Generate(StatementContext context) {
        ExpressionHelp.Generate(left, context);
        ExpressionHelp.Generate(right, context);
        left.GetResultType().GenerateLessEqual(position, context);
    }
}
