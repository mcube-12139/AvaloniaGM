using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Type;

public class IntegerType : IType {
    string IType.GetAppearance() => "int";

    bool IType.EqualType(IType other) {
        return other == this;
    }

    Intermediate.Type.IType IType.ToIntermediate() {
        return Intermediate.Type.PrimitiveType.INTEGER;
    }

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
        right.SetTargetType(this);
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

    void IType.GenerateMultiply(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(MultiplyInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateDivide(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(DivideInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateModulo(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(ModuloInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateAdd(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(AddInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateSubtract(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(SubtractInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateLeftShift(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(LeftShiftInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateRightShift(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(RightShiftInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateBitAnd(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(BitAndInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateBitXor(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(BitXorInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateBitOr(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(BitOrInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateGreater(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(GreaterInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateGreaterEqual(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(GreaterEqualInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateLess(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(LessInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateLessEqual(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(LessEqualInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateEqual(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(EqualInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateNotEqual(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(NotEqualInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateAddAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(AddInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateSubtractAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(SubtractInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateMultiplyAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(MultiplyInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateDivideAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(DivideInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateModuloAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(ModuloInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateBitAndAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(BitAndInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateBitOrAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(BitOrInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateBitXorAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(BitXorInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateLeftShiftAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(LeftShiftInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }

    void IType.GenerateRightShiftAssign(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(RightShiftInstruction.Instantiate(Intermediate.Type.PrimitiveType.INTEGER), position);
    }
}
