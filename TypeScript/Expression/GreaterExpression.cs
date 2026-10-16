using TypeScript.Type;

namespace TypeScript.Expression;

public class GreaterExpression(TextPosition position, IExpression left, IExpression right) : IExpression {
    public readonly TextPosition position = position;
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanGreater(right, position, compiler);
    }
}
