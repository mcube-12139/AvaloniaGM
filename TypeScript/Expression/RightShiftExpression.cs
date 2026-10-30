using TypeScript.Type;

namespace TypeScript.Expression;

public class RightShiftExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    IType? type;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        type = left.GetResultType().GetRightShiftResultType(right, position, compiler);
    }
}

