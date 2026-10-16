using TypeScript.Type;

namespace TypeScript.Expression;

public class StringExpression(TextPosition position, string value) : IExpression {
    public readonly TextPosition position = position;
    public readonly string value = value;


    TextPosition IExpression.GetPosition() => position;

    IType IExpression.GetResultType() => PrimitiveType.STRING;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
    }
}
