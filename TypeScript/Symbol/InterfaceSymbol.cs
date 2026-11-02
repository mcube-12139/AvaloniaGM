using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class InterfaceSymbol(string name, InterfaceItem item) : ITypeSymbol, IConceptSymbol {
    public readonly string name = name;
    internal InterfaceItem item = item;

    internal InterfaceType type = new(name);

    string ISymbol.GetName() => name;

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;

    Dictionary<string, ConceptFunction> IConceptSymbol.GetFunctions(Compiler compiler) {
        throw new NotImplementedException();
    }
}
