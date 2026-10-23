using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class InterfaceSymbol(string name, InterfaceItem item) : ITypeSymbol {
    public readonly string name = name;
    internal InterfaceItem item = item;

    internal InterfaceType type = new(name);
    internal Dictionary<string, FunctionSymbol> conceptFunctions = [];

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position) => type.space;

    Dictionary<string, FunctionSymbol> ISymbol.GetConceptFunctions(SourcePosition position) => conceptFunctions;

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
