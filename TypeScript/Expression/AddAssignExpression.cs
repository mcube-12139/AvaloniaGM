using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Expression;

public class AddAssignExpression(SourcePosition position, IPlaceExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IPlaceExpression left = left;
    public readonly IExpression right = right;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => TupleType.EMPTY;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanAddAssign(right, position, compiler);
    }

    void IExpression.Generate(StatementContext context) {
        left.GenerateCompoundAssignStart(context);
        ExpressionHelp.Generate(right, context);
        left.GetResultType().GenerateAddAssign(position, context);
        left.GenerateCompoundAssign(context);
    }
}
