using TypeScript.Type;

namespace TypeScript.Expression;

public class BitXorAssignExpression(TextPosition position, IPlaceExpression left, IExpression right) : IExpression {
    public readonly TextPosition position = position;
    public readonly IPlaceExpression left = left;
    public readonly IExpression right = right;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => TupleType.EMPTY;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanBitXorAssign(right, position, compiler);
    }
}
