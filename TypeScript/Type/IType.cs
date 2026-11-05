using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.GenerateContext;
using TypeScript.Item;
using TypeScript.Symbol;

namespace TypeScript.Type;

public interface IType {
    string GetAppearance();
    bool EqualType(IType other);
    bool IsType(IType other) {
        return EqualType(other);
    }
    internal Intermediate.Type.IType ToIntermediate() => throw new NotImplementedException();

    void AddImpl(ISymbol symbol, IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.EXTERNAL_TYPE_IMPLEMENTED, [GetAppearance()], position);
    }

    void SetImplMethods(IType type, FunctionSymbol[] methods, SourcePosition position) {
        throw new SemanticException(SemanticErrorType.EXTERNAL_TYPE_IMPLEMENTED, [GetAppearance()], position);
    }

    void AddFunctions(IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.EXTERNAL_TYPE_IMPLEMENTED, [GetAppearance()], position);
    }

    FunctionType AsFunctionType(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_CALLABLE, [GetAppearance()], position);
    }

    ISymbol GetAssociatedSymbol(string name, SourcePosition position) {
        throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [GetAppearance(), name], position);
    }

    IField GetField(string name, SourcePosition position) {
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

    internal void GenerateSetIndex(SourcePosition position, StatementContext context) { }
    internal void GenerateGetIndex(SourcePosition position, StatementContext context) { }
    internal void GenerateMultiply(SourcePosition position, StatementContext context) { }
    internal void GenerateDivide(SourcePosition position, StatementContext context) { }
    internal void GenerateModulo(SourcePosition position, StatementContext context) { }
    internal void GenerateAdd(SourcePosition position, StatementContext context) { }
    internal void GenerateSubtract(SourcePosition position, StatementContext context) { }
    internal void GenerateLeftShift(SourcePosition position, StatementContext context) { }
    internal void GenerateRightShift(SourcePosition position, StatementContext context) { }
    internal void GenerateBitAnd(SourcePosition position, StatementContext context) { }
    internal void GenerateBitXor(SourcePosition position, StatementContext context) { }
    internal void GenerateBitOr(SourcePosition position, StatementContext context) { }
    internal void GenerateGreater(SourcePosition position, StatementContext context) { }
    internal void GenerateGreaterEqual(SourcePosition position, StatementContext context) { }
    internal void GenerateLess(SourcePosition position, StatementContext context) { }
    internal void GenerateLessEqual(SourcePosition position, StatementContext context) { }
    internal void GenerateEqual(SourcePosition position, StatementContext context) { }
    internal void GenerateNotEqual(SourcePosition position, StatementContext context) { }
    internal void GenerateAddAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateSubtractAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateMultiplyAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateDivideAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateModuloAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateBitAndAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateBitOrAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateBitXorAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateLeftShiftAssign(SourcePosition position, StatementContext context) { }
    internal void GenerateRightShiftAssign(SourcePosition position, StatementContext context) { }
}
