using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class StringType : IType {
    string IType.GetAppearance() => "string";

    bool IType.IsType(IType other) {
        return other == this;
    }

    IType IType.GetAddResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string + {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    void IType.EnsureCanGreater(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string > {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanGreaterEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string >= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLess(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string < {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLessEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string <= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string = {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAddAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string += {rightType.GetAppearance()}"], position);
        }
    }
}
