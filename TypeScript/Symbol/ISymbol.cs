using TypeScript.ExceptionSpace;

namespace TypeScript.Symbol;

public interface ISymbol {
    string GetName();
    internal IValueSymbol AsValue(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_VALUE, [GetName()], position);
    }
    internal ITypeSymbol AsType(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_TYPE, [GetName()], position);
    }
    internal NameSpace GetNameSpace(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NO_NAME_SPACE, [GetName()], position);
    }
}
