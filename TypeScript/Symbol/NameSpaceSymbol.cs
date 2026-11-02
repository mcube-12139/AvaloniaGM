using TypeScript.NameSpace;

namespace TypeScript.Symbol;

public class NameSpaceSymbol : ISymbol {
    public readonly string name;
    internal readonly SymbolSpace space;

    internal NameSpaceSymbol(string name, SymbolSpace space) {
        this.name = name;
        this.space = space;
    }

    string ISymbol.GetName() => name;

    SymbolSpace ISymbol.GetNameSpace(SourcePosition position) => space;
}
