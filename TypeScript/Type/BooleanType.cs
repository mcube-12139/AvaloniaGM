using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class BooleanType : IType {
    string IType.GetAppearance() => "bool";

    bool IType.IsType(IType other) {
        return other == this;
    }

    void IType.EnsureCanEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"bool == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"bool != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"bool = {rightType.GetAppearance()}"], position);
        }
    }
}
