using TypeScript.Type;

namespace TypeScript.Expression;

public class LeftShiftExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        type = left.GetResultType().GetLeftShiftResultType(right, position, compiler);
    }
}
