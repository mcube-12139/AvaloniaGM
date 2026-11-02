using TypeScript.NameSpace;
using TypeScript.Symbol;

namespace TypeScript.Type;

internal class TypeHead {
    internal SymbolSpace space = new(null);
    internal Dictionary<IConceptSymbol, SymbolSpace> impls = [];
}
