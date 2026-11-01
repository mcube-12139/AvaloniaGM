using TypeScript.Item;

namespace TypeScript.Symbol;

public class ConceptSymbol : IConceptSymbol {
    public readonly string name;
    public readonly ConceptItem item;

    internal ConceptSymbol(string name, ConceptItem item) {
        this.name = name;
        this.item = item;
    }

    internal Dictionary<string, GenericFunctionSymbol> conceptFunctions = [];

    string ISymbol.GetName() => name;

    Dictionary<string, GenericFunctionSymbol> IConceptSymbol.GetFunctions() => conceptFunctions;
}
