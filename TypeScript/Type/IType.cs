using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.GenericArgument;
using TypeScript.NameSpace;
using TypeScript.Symbol;

namespace TypeScript.Type;

public interface IType: IGenericArgument {
    internal TypeHead GetHead() {
        throw new NotImplementedException();
    }
    internal SymbolSpace GetNameSpace() => GetHead().space;
    internal void AddInherentImpl(ISymbol symbol, SourcePosition position) {
        TypeHead head = GetHead();
        head.space.AddSymbol(symbol.GetName(), symbol, position);
    }
    internal void AddConceptImpl(IConceptSymbol conceptSymbol, IEnumerable<ISymbol> symbols, SourcePosition position) {
        TypeHead head = GetHead();
        SymbolSpace implSpace = new(null);
        foreach (ISymbol symbol in symbols) {
            head.space.AddSymbol(symbol.GetName(), symbol, position);
            implSpace.AddSymbol(symbol.GetName(), symbol, position);
        }
        head.impls.Add(conceptSymbol, implSpace);
    }
    ISymbol GetImplSymbol(IConceptSymbol conceptSymbol, string name) {
        return GetHead().impls[conceptSymbol].GetSymbolOrException(name);
    }

    IType IGenericArgument.AsType(SourcePosition position) => this;

    IType GetConcreteType() {
        return this;
    }

    /// <summary>
    /// 如果是泛型且被代入，则返回代入类型；否则，返回自身。
    /// </summary>
    /// <returns></returns>
    IType TryGetConcreteType() {
        return this;
    }

    internal void InferGenericByArgument(IExpression argument, GenericInferContext context) {
        argument.SetSemantic(this, context.compiler);

        IType parameterType = argument.GetResultType();
        if (!TypeHelp.IsType(parameterType, this)) {
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, [parameterType.GetAppearance(), GetAppearance()], context.position);
        }
    }

    internal void InferGenericByTarget(IType? target, GenericInferContext context) {
        if (target != null && !TypeHelp.IsType(this, target)) {
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, [GetAppearance(), target.GetAppearance()], context.position);
        }
    }

    bool IsType(IType other) {
        return other == this;
    }

    FunctionType AsFunctionType(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_CALLABLE, [GetAppearance()], position);
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
