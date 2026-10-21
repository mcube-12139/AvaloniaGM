using TypeScript.ExceptionSpace;
using TypeScript.Symbol;

namespace TypeScript.PathSegment;

public class ThisSegment: IPathSegment {
    public readonly SourcePosition position;

    internal ThisSegment(SourcePosition position) {
        this.position = position;
    }

    ISymbol IPathSegment.GetSymbol(Compiler compiler) {
        if (compiler.thisParameterSymbol != null) {
            return compiler.thisParameterSymbol;
        }

        throw new SemanticException(SemanticErrorType.INVALID_THIS, [], position);
    }

    ISymbol IPathSegment.GetSymbolInNameSpace(NameSpace space) {
        throw new SemanticException(SemanticErrorType.INVALID_THIS, [], position);
    }
}
