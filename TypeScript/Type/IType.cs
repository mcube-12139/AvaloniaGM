using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.GenericArgument;
using TypeScript.Item;
using TypeScript.Symbol;

namespace TypeScript.Type;

public interface IType: IGenericArgument {
    IType IGenericArgument.AsType(SourcePosition position) => this;

    IType GetConcreteType() {
        return this;
    }

    bool IsType(IType other) {
        return other == this;
    }

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
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()}[?]"], position);
    }

    IType GetMultiplyResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} * ?"], position);
    }

    IType GetDivideResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} / ?"], position);
    }

    IType GetModuloResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} % ?"], position);
    }

    IType GetAddResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} + ?"], position);
    }

    IType GetSubtractResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} - ?"], position);
    }

    IType GetLeftShiftResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} << ?"], position);
    }

    IType GetRightShiftResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >> ?"], position);
    }

    IType GetBitAndResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} & ?"], position);
    }

    IType GetBitXorResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} ^ ?"], position);
    }

    IType GetBitOrResultType(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} | ?"], position);
    }

    void EnsureCanGreater(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} > ?"], position);
    }

    void EnsureCanGreaterEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >= ?"], position);
    }

    void EnsureCanLess(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} < ?"], position);
    }

    void EnsureCanLessEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} <= ?"], position);
    }

    void EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} == ?"], position);
    }

    void EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} != ?"], position);
    }

    void EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} = ?"], position);
    }

    void EnsureCanAddAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} += ?"], position);
    }

    void EnsureCanSubtractAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} -= ?"], position);
    }

    void EnsureCanMultiplyAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} *= ?"], position);
    }

    void EnsureCanDivideAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} /= ?"], position);
    }

    void EnsureCanModuloAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} %= ?"], position);
    }

    void EnsureCanBitAndAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} &= ?"], position);
    }

    void EnsureCanBitOrAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} |= ?"], position);
    }

    void EnsureCanBitXorAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} ^= ?"], position);
    }

    void EnsureCanLeftShiftAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} <<= ?"], position);
    }

    void EnsureCanRightShiftAssign(IExpression right, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} >>= ?"], position);
    }
}
