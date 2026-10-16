using TypeScript.Type;

namespace TypeScript.Expression;

public class BooleanExpression(TextPosition position, bool value) : IExpression {
    public readonly TextPosition position = position;
    public readonly bool value = value;


    TextPosition IExpression.GetPosition() => position;

    IType IExpression.GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
    }
}
