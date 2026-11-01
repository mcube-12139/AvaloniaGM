using TypeScript.ExceptionSpace;
using TypeScript.Symbol;

namespace TypeScript.PathSegment;

public class UpperThisSegment: IPathSegment {
    public readonly SourcePosition position;

    internal UpperThisSegment(SourcePosition position) {
        this.position = position;
    }

    ISymbol IPathSegment.GetSymbol(Compiler compiler) {
        if (compiler.thisTypeSymbol != null) {
            return compiler.thisTypeSymbol;
        }

        throw new SemanticException(SemanticErrorType.INVALID_UPPER_THIS, [], position);
    }

    ISymbol IPathSegment.GetSymbolInNameSpace(NameSpace space) {
        throw new SemanticException(SemanticErrorType.INVALID_UPPER_THIS, [], position);
    }
}
