using TypeScript.Type;

namespace TypeScript.Expression;

public class NotEqualExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanNotEqual(right, position, compiler);
    }
}
