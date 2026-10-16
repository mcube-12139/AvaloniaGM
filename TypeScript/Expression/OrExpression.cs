using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public class OrExpression(TextPosition position, IExpression left, IExpression right) : IExpression {
    public readonly TextPosition position = position;
    public readonly IExpression left = left;
    public readonly IExpression right = right;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => PrimitiveType.BOOLEAN;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        left.SetSemantic(PrimitiveType.BOOLEAN, compiler);
        right.SetSemantic(PrimitiveType.BOOLEAN, compiler);
        IType leftType = left.GetResultType();
        IType rightType = right.GetResultType();
        if (leftType != PrimitiveType.BOOLEAN || rightType != PrimitiveType.BOOLEAN) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{leftType.GetAppearance()} || {rightType.GetAppearance()}"], position);
        }
    }
}
