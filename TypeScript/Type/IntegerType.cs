using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class IntegerType : IType {
    string IType.GetAppearance() => "int";

    bool IType.IsType(IType other) {
        return other == this;
    }

    IType IType.GetNegativeResultType(TextPosition position, Compiler compiler) {
        return this;
    }

    IType IType.GetNotResultType(TextPosition position, Compiler compiler) {
        return this;
    }

    IType IType.GetMultiplyResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int * {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetDivideResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int / {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetModuloResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int % {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetAddResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetSubtractResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int - {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetLeftShiftResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int << {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetRightShiftResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >> {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetBitAndResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int & {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetBitXorResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int ^ {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetBitOrResultType(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int | {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    void IType.EnsureCanGreater(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int > {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanGreaterEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLess(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int < {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLessEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int <= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int = {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAddAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int += {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanSubtractAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int -= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanMultiplyAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int *= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanDivideAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int /= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanModuloAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int %= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanBitAndAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int &= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanBitOrAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int |= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanBitXorAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int ^= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLeftShiftAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int <<= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanRightShiftAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >>= {rightType.GetAppearance()}"], position);
        }
    }
}
