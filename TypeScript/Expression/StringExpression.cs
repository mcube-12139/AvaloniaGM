using TypeScript.Type;

namespace TypeScript.Expression;

public class StringExpression(SourcePosition position, string value) : IExpression {
    public readonly SourcePosition position = position;
    public readonly string value = value;


    SourcePosition IExpression.GetPosition() => position;

    IType IExpression.GetResultType() => PrimitiveType.STRING;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
    }
}
