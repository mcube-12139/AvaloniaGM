using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Symbol;

public interface ISymbol {
    string GetName();
    internal IValueSymbol AsValue(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_VALUE, [GetName()], position);
    }
    internal ITypeSymbol AsType(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_TYPE, [GetName()], position);
    }
    internal NameSpace GetNameSpace(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NO_NAME_SPACE, [GetName()], position);
    }
}
