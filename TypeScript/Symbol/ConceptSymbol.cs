using TypeScript.Item;
using TypeScript.NameSpace;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ConceptSymbol : IConceptSymbol {
    public readonly string name;
    internal readonly SpaceState itemState;
    public readonly ConceptItem item;

    internal bool resolved = false;
    internal ThisTypeParameterSymbol? thisTypeSymbol;

    internal ConceptSymbol(string name, SpaceState itemState, ConceptItem item) {
        this.name = name;
        this.itemState = itemState;
        this.item = item;
    }

    internal Dictionary<string, ConceptFunction> conceptFunctions = [];

    string ISymbol.GetName() => name;

    Dictionary<string, ConceptFunction> IConceptSymbol.GetFunctions(Compiler compiler) {
        Resolve(compiler);
        return conceptFunctions;
    }

    void Resolve(Compiler compiler) {
        if (!resolved) {
            compiler.PushSpaceState(itemState);
            item.Resolve(compiler);
            compiler.PopSpaceState();
        }
    }

    void IConceptSymbol.SetThisType(IType type, Compiler compiler) {
        Resolve(compiler);
        thisTypeSymbol!.type.concreteType = type;
    }
}
