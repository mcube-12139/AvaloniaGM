using TypeScript.Field;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ClassSymbol(string name, ClassItem item) : ITypeSymbol {
    public readonly string name = name;
    internal ClassItem item = item;

    internal ClassType? type;

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position) => type!.space;

    IType ITypeSymbol.GetSelfType() => type!;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) {
        if (type == null) {
            StructField[] fields = item.ResolveFields(compiler);

            type = new(name, fields);
        }

        return type;
    }
}
