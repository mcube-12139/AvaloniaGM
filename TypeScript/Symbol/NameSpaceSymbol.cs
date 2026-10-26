namespace TypeScript.Symbol;

public class NameSpaceSymbol : ISymbol {
    public readonly string name;
    internal readonly NameSpace space;

    internal NameSpaceSymbol(string name, NameSpace space) {
        this.name = name;
        this.space = space;
    }

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position) => space;
}
