using TypeScript.Symbol;

namespace TypeScript.PathSegment;

class IdentifierSegment: IPathSegment {
    public readonly SourcePosition position;
    public readonly string name;

    internal IdentifierSegment(SourcePosition position, string name) {
        this.position = position;
        this.name = name;
    }

    ISymbol IPathSegment.GetSymbol(Compiler compiler) {
        return compiler.GetSymbol(name, position);
    }

    ISymbol IPathSegment.GetSymbolInNameSpace(NameSpace space) {
        return space.GetSymbol(name, position);
    }
}
