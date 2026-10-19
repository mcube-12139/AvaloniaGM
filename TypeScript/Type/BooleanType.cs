using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class BooleanType : IType {
    string IType.GetAppearance() => "boolean";

    bool IType.EqualType(IType other) {
        return other == this;
    }

    IType IType.GetNotResultType(SourcePosition position, Compiler compiler) {
        return this;
    }

    void IType.EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"boolean == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"boolean != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"boolean = {rightType.GetAppearance()}"], position);
        }
    }
}
