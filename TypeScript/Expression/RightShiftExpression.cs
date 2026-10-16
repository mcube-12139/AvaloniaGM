using TypeScript.Type;

namespace TypeScript.Expression;

public class RightShiftExpression(TextPosition position, IExpression left, IExpression right) : IExpression {
    public readonly TextPosition position = position;
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    IType? type;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        type = left.GetResultType().GetRightShiftResultType(right, position, compiler);
    }
}
