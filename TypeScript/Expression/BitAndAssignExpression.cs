using TypeScript.Type;

namespace TypeScript.Expression;

public class BitAndAssignExpression(SourcePosition position, IPlaceExpression left, IExpression right) : IExpression {
    public readonly SourcePosition position = position;
    public readonly IPlaceExpression left = left;
    public readonly IExpression right = right;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => TupleType.EMPTY;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanBitAndAssign(right, position, compiler);
    }
}
