using TypeScript.ExceptionSpace;

namespace TypeScript.Symbol;

public interface ISymbol {
    string GetName();
    IValueSymbol AsValue(TextPosition position, Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.NOT_VALUE, [GetName()], position);
    }
    //ITypeSymbol AsType(TextPosition position, Generator generator);
}
