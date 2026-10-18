using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public interface IType {
    string GetAppearance();
    bool IsType(IType other);
    FunctionType AsFunctionType(TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.NOT_CALLABLE, [GetAppearance()], position);
    }

    IType GetNegativeResultType(TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"-{GetAppearance()}"], position);
    }

    IType GetNotResultType(TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"!{GetAppearance()}"], position);
    }

    IType GetIndexResultType(IExpression index, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()}[{index.GetResultType().GetAppearance()}]"], position);
    }

    IType GetMultiplyResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} * {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetDivideResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} / {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetModuloResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} % {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetAddResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} + {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetSubtractResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} - {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetLeftShiftResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} << {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetRightShiftResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >> {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetBitAndResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} & {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetBitXorResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} ^ {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetBitOrResultType(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} | {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanGreater(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} > {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanGreaterEqual(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanLess(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} < {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanLessEqual(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} <= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanEqual(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} == {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanNotEqual(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} != {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} = {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanAddAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} += {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanSubtractAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} -= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanMultiplyAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} *= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanDivideAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} /= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanModuloAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} %= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanBitAndAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} &= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanBitOrAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} |= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanBitXorAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} ^= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanLeftShiftAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} <<= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanRightShiftAssign(IExpression right, TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >>= {right.GetResultType().GetAppearance()}"], position);
    }
}
