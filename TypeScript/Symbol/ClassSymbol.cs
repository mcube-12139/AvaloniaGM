using TypeScript.Field;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ClassSymbol : ITypeSymbol {
    public readonly string name;
    internal readonly NameSpace itemSpace;
    internal ClassItem item;

    internal ClassSymbol(string name, NameSpace itemSpace, ClassItem item) {
        this.name = name;
        this.itemSpace = itemSpace;
        this.item = item;
    }

    internal ClassType? type;

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position) => type!.space;

    IType ITypeSymbol.GetSelfType() => type!;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) {
        if (type == null) {
            compiler.PushNameSpace(itemSpace);
            StructField[] fields = item.ResolveFields(compiler);
            compiler.PopNameSpace();

            type = new(name, fields);
        }

        return type;
    }
}
