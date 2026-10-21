using TypeScript.Symbol;

namespace TypeScript.PathSegment;

public interface IPathSegment {
    internal ISymbol GetSymbol(Compiler compiler);
    internal ISymbol GetSymbolInNameSpace(NameSpace space);
}
