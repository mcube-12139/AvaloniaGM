using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ClassSymbol(string name, ClassItem item) : ITypeSymbol {
    public readonly string name = name;
    internal ClassItem item = item;
    internal NameSpace space = new();

    internal ClassType? type;

    string ISymbol.GetName() => name;

    NameSpace ISymbol.GetNameSpace(SourcePosition position) => space;

    IType ITypeSymbol.GetSelfType() => type!;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) {
        if (type == null) {
            ClassType.Field[] fields = item.ResolveFields(compiler);

            type = new(name, fields);
        }

        return type;
    }

    void ITypeSymbol.AddImplSymbol(IType? regardedType, FunctionItem[] functions, SourcePosition position, Compiler compiler) {
        // 在类的名字空间里新增符号
        compiler.EnterNameSpace(space);
        foreach (FunctionItem funItem in functions) {
            funItem.AddImplSymbol(compiler);
        }
        compiler.LeaveNameSpace();
    }
}
