using TypeScript.Expression;
using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IValueSymbol ISymbol.AsValue(SourcePosition position) {
        return this;
    }
    IType GetValueType();
    internal IType LazyGetValueType(SourcePosition position, Compiler compiler);
    internal void Store(SourcePosition position, Compiler compiler);
    internal void EnsureCanLoad(SourcePosition position);
    internal void GenerateCall(CallExpression call, StatementContext context) => throw new NotImplementedException();
}
