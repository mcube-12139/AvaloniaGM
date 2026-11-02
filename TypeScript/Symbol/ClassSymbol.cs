using TypeScript.Field;
using TypeScript.Item;
using TypeScript.NameSpace;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ClassSymbol : ITypeSymbol {
    public readonly string name;
    internal readonly SpaceState itemState;
    internal ClassItem item;

    internal ClassSymbol(string name, SpaceState itemState, ClassItem item) {
        this.name = name;
        this.itemState = itemState;
        this.item = item;
    }

    internal ClassType? type;

    string ISymbol.GetName() => name;

    SymbolSpace ISymbol.GetNameSpace(SourcePosition position) => ((IType)type!).GetNameSpace();

    IType ITypeSymbol.GetSelfType() => type!;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) {
        if (type == null) {
            compiler.PushSpaceState(itemState);
            StructField[] fields = item.ResolveFields(compiler);
            compiler.PopSpaceState();

            type = new(name, fields);
        }

        return type;
    }
}
