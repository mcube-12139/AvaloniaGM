using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public class OrExpression(SourcePosition position, IExpression left, IExpression right) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(PrimitiveType.BOOLEAN, compiler);
        right.SetSemantic(PrimitiveType.BOOLEAN, compiler);
        IType leftType = left.GetResultType();
        IType rightType = right.GetResultType();
        if (leftType != PrimitiveType.BOOLEAN || rightType != PrimitiveType.BOOLEAN) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{leftType.GetAppearance()} || {rightType.GetAppearance()}"], position);
        }
    }
}

