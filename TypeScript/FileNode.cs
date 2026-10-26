using TypeScript.Item;

namespace TypeScript;

public class FileNode(IItem[] items, ImplNode[] impls) {
    public readonly IItem[] items = items;
    public readonly ImplNode[] impls = impls;

    internal void AddItemSymbol(Compiler compiler) {
        foreach (IItem item in items) {
            item.AddItemSymbol(compiler);
        }
    }

    internal void AddImplSymbol(Compiler compiler) {
        foreach (IItem item in items) {
            item.AddImplSymbol(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.AddSymbol(compiler);
        }
    }

    internal void SetSemantic(Compiler compiler) {
        foreach (IItem item in items) {
            item.SetSemantic(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.SetSemantic(compiler);
        }
    }
}
