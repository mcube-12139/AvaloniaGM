using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IValueSymbol ISymbol.AsValue(SourcePosition position) {
        return this;
    }
    IValueSymbol ISymbol.GetGenericOmittedCallSymbol(IType? target, IExpression[] parameters, SourcePosition position, Compiler compiler) {
        LazyGetValueType(position, compiler).AsFunctionType(position, compiler).EnsureCanCall(parameters, position, compiler);
        return this;
    }
    IType GetValueType();
    internal IType LazyGetValueType(SourcePosition position, Compiler compiler);
    internal void Store(SourcePosition position, Compiler compiler);
    internal void EnsureCanLoad(SourcePosition position);
}
