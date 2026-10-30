using TypeScript.Type;

namespace TypeScript.Expression;

public class StringExpression(SourcePosition position, string value) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly string value = value;


    ExpressionHead IExpression.GetHead() => head;

    IType IExpression.GetResultType() => PrimitiveType.STRING;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        // 无事发生
    }
}

