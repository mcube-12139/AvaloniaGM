using TypeScript.Type;

namespace TypeScript.Expression;

public class IndexExpression(TextPosition position, IExpression indexed, IExpression index) : IPlaceExpression {
    public readonly TextPosition position = position;
    public readonly IExpression indexed = indexed;
    public readonly IExpression index = index;

    IType? type;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        indexed.SetSemantic(null, compiler);
        type = indexed.GetResultType().GetIndexResultType(index, position, compiler);
    }
}
