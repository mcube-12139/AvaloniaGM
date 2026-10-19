using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ClassSymbol(string name, ClassItem item) : ITypeSymbol {
    public readonly string name = name;
    internal ClassItem item = item;
    internal NameSpace space = new();

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position, Compiler compiler) => space;

    IType ITypeSymbol.GetSelfType() {
        throw new NotImplementedException();
    }
}
