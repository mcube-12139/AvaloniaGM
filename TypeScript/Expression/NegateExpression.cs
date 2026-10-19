using TypeScript.Type;

namespace TypeScript.Expression;

public class NegateExpression(SourcePosition position, IExpression expression) : IExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression expression = expression;

    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        expression.SetSemantic(null, compiler);
        type = expression.GetResultType().GetNegativeResultType(position, compiler);
    }
}
