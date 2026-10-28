using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class InterfaceSymbol(string name, InterfaceItem item) : ITypeSymbol, IConceptSymbol {
    public readonly string name = name;
    internal InterfaceItem item = item;

    internal InterfaceType type = new(name);
    internal Dictionary<string, ConceptFunctionSymbol> conceptFunctions = [];

    string ISymbol.GetName() => name;

    Dictionary<string, ConceptFunctionSymbol> ISymbol.GetConceptFunctions(SourcePosition position) => conceptFunctions;

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;
}
