using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Expression;

public class MultiplyAssignExpression(SourcePosition position, IPlaceExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IPlaceExpression left = left;
    public readonly IExpression right = right;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => TupleType.EMPTY;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(null, compiler);
        left.GetResultType().EnsureCanMultiplyAssign(right, position, compiler);
    }

    void IExpression.Generate(StatementContext context) {
        left.GenerateCompoundAssignStart(context);
        ExpressionHelp.Generate(right, context);
        left.GetResultType().GenerateMultiplyAssign(position, context);
        left.GenerateCompoundAssign(context);
    }
}
