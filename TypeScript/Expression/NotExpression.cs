using TypeScript.Type;

namespace TypeScript.Expression;

public class NotExpression(TextPosition position, IExpression expression) : IExpression {
    public readonly TextPosition position = position;
    public readonly IExpression expression = expression;

    IType? type;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        expression.SetSemantic(null, compiler);
        type = expression.GetResultType().GetNotResultType(position, compiler);
    }
}
