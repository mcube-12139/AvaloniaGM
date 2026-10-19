using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class StringType : IType {
    string IType.GetAppearance() => "string";

    bool IType.EqualType(IType other) {
        return other == this;
    }

    IType IType.GetAddResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string + {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    void IType.EnsureCanGreater(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string > {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanGreaterEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string >= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLess(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string < {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLessEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string <= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string = {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAddAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string += {rightType.GetAppearance()}"], position);
        }
    }
}
