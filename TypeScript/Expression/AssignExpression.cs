using TypeScript.Type;

namespace TypeScript.Expression;

public class AssignExpression(SourcePosition position, IPlaceExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IPlaceExpression left = left;
    public readonly IExpression right = right;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => TupleType.EMPTY;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetAssignSemantic(compiler);
        left.GetResultType().EnsureCanAssign(right, position, compiler);
    }
}

