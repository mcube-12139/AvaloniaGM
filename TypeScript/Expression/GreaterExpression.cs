using TypeScript.Type;

namespace TypeScript.Expression;

public class GreaterExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanGreater(right, position, compiler);
    }
}

