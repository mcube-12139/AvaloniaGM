using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Symbol;

namespace TypeScript.Type;

public interface IType {
    string GetAppearance();
    bool EqualType(IType other);
    bool IsType(IType other) {
        return EqualType(other);
    }

    void AddImpl(ISymbol symbol, IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.EXTERNAL_TYPE_IMPLEMENTED, [GetAppearance()], position);
    }

    void AddFunctions(IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.EXTERNAL_TYPE_IMPLEMENTED, [GetAppearance()], position);
    }

    FunctionType AsFunctionType(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_CALLABLE, [GetAppearance()], position);
    }

    ISymbol GetImplItemSymbol(string name, SourcePosition position) {
        throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [GetAppearance(), name], position);
    }

    IType GetFieldAccessResultType(string name, SourcePosition position) {
        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [GetAppearance(), name], position);
    }

    IType GetNegativeResultType(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"-{GetAppearance()}"], position);
    }

    IType GetNotResultType(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"!{GetAppearance()}"], position);
    }

    IType GetIndexResultType(IExpression index, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()}[{index.GetResultType().GetAppearance()}]"], position);
    }

    IType GetMultiplyResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} * {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetDivideResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} / {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetModuloResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} % {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetAddResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} + {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetSubtractResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} - {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetLeftShiftResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} << {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetRightShiftResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >> {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetBitAndResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} & {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetBitXorResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} ^ {right.GetResultType().GetAppearance()}"], position);
    }

    IType GetBitOrResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} | {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanGreater(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} > {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanGreaterEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanLess(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} < {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanLessEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} <= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} == {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} != {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} = {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanAddAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} += {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanSubtractAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} -= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanMultiplyAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} *= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanDivideAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} /= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanModuloAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} %= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanBitAndAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} &= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanBitOrAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} |= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanBitXorAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} ^= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanLeftShiftAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} <<= {right.GetResultType().GetAppearance()}"], position);
    }

    void EnsureCanRightShiftAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >>= {right.GetResultType().GetAppearance()}"], position);
    }
}
