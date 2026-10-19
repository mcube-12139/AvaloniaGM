using TypeScript.Type;

namespace TypeScript.Expression;

public class BitAndExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        type = left.GetResultType().GetBitAndResultType(right, position, compiler);
    }
}
