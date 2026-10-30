using TypeScript.Type;

namespace TypeScript.Expression;

public class NegateExpression(SourcePosition position, IExpression expression) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression expression = expression;

    IType? type;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        expression.SetSemantic(null, compiler);
        type = expression.GetResultType().GetNegativeResultType(position, compiler);
    }
}

