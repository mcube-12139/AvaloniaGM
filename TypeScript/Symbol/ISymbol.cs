using TypeScript.ExceptionSpace;

namespace TypeScript.Symbol;

public interface ISymbol {
    string GetName();
    internal IValueSymbol AsValue(TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.NOT_VALUE, [GetName()], position);
    }
    internal ITypeSymbol AsType(TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.NOT_TYPE, [GetName()], position);
    }
}
