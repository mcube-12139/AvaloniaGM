using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ClassSymbol(string name, ClassItem item) : ITypeSymbol {
    public readonly string name = name;
    internal ClassItem item = item;
    internal NameSpace space = new();

    ClassType? type;

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position) => space;

    IType ITypeSymbol.GetSelfType() => type!;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) {
        throw new NotImplementedException();
    }
}
