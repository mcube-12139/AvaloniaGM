using TypeScript.Type;

namespace TypeScript.Expression;

public class IndexExpression(SourcePosition position, IExpression indexed, IExpression index) : IPlaceExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression indexed = indexed;
    public readonly IExpression index = index;

    IType? type;

    ExpressionHead IExpression.GetHead() => head;

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

