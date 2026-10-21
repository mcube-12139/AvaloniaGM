using TypeScript.ExceptionSpace;
using TypeScript.Expression;
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
    internal IType GetMethodCallResultType(IExpression[] parameterTypes, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.METHOD_NOT_EXIST, [GetName()], position);
    }
}
