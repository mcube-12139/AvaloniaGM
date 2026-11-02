using TypeScript.Symbol;

namespace TypeScript.NameSpace;

internal interface INameSpace {
    INameSpace? GetUpper();
    void AddSymbol(string name, ISymbol symbol, SourcePosition position);
    ISymbol? TryGetSymbol(string name);
    ISymbol GetSymbol(string name, SourcePosition position);
    ISymbol GetSymbolOrException(string name);
}
