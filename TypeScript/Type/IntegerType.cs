using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class IntegerType : IType {
    string IType.GetAppearance() => "int";

    IType IType.GetNegativeResultType(SourcePosition position, Compiler compiler) {
        return this;
    }

    IType IType.GetNotResultType(SourcePosition position, Compiler compiler) {
        return this;
    }

    IType IType.GetMultiplyResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int * {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetDivideResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int / {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetModuloResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int % {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetAddResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetSubtractResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int - {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetLeftShiftResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int << {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetRightShiftResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >> {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetBitAndResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int & {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetBitXorResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int ^ {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    IType IType.GetBitOrResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int | {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    void IType.EnsureCanGreater(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int > {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanGreaterEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLess(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int < {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLessEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int <= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int = {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAddAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int += {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanSubtractAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int -= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanMultiplyAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int *= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanDivideAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int /= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanModuloAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int %= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanBitAndAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int &= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanBitOrAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int |= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanBitXorAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int ^= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLeftShiftAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int <<= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanRightShiftAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >>= {rightType.GetAppearance()}"], position);
        }
    }
}
