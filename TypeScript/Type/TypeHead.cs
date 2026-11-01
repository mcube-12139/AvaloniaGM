using TypeScript.Symbol;

namespace TypeScript.Type;

internal class TypeHead {
    internal NameSpace space = new(null);
    internal Dictionary<IConceptSymbol, NameSpace> impls = [];
}
