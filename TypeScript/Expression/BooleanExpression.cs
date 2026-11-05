using TypeScript.Type;

namespace TypeScript.Expression;

public class BooleanExpression(SourcePosition position, bool value) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly bool value = value;


    ExpressionHead IExpression.GetHead() => head;

    IType IExpression.GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
    }
}

