using TypeScript.Symbol;

namespace TypeScript.Item;

public class NameSpaceItem(SourcePosition position, string name, IItem[] items, ImplNode[] impls) : IItem {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly IItem[] items = items;
    public readonly ImplNode[] impls = impls;

    NameSpaceSymbol? symbol;
    
    void IItem.AddItemSymbol(Compiler compiler) {
        NameSpace space = compiler.EnterNewNameSpace();
        symbol = new NameSpaceSymbol(name, space);

        foreach (IItem item in items) {
            item.AddItemSymbol(compiler);
        }

        compiler.LeaveNameSpace();
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.AddImplSymbol(Compiler compiler) {
        compiler.space = symbol!.space;

        foreach (IItem item in items) {
            item.AddImplSymbol(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.AddSymbol(compiler);
        }

        compiler.LeaveNameSpace();
    }

    void IItem.SetSemantic(Compiler compiler) {
        compiler.space = symbol!.space;

        foreach (IItem item in items) {
            item.SetSemantic(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.SetSemantic(compiler);
        }

        compiler.LeaveNameSpace();
    }
}
