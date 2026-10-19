using TypeScript.Type;

namespace TypeScript.Expression;

public class BooleanExpression(SourcePosition position, bool value) : IExpression {
    public readonly SourcePosition position = position;
    public readonly bool value = value;


    SourcePosition IExpression.GetPosition() => position;

    IType IExpression.GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
    }
}
