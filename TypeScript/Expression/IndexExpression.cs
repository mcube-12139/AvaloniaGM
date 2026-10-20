using TypeScript.Type;

namespace TypeScript.Expression;

public class IndexExpression(SourcePosition position, IExpression indexed, IExpression index) : IPlaceExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression indexed = indexed;
    public readonly IExpression index = index;

    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        indexed.SetSemantic(null, compiler);
        type = indexed.GetResultType().GetIndexResultType(index, position, compiler);
    }

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        indexed.SetSemantic(null, compiler);
        type = indexed.GetResultType().GetIndexResultType(index, position, compiler);
    }
}
