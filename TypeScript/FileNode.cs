using TypeScript.Item;

namespace TypeScript;

public class FileNode(IItem[] items, ImplNode[] impls) {
    public readonly IItem[] items = items;
    public readonly ImplNode[] impls = impls;

    internal void SetSemantic(Compiler compiler) {
        foreach (IItem item in items) {
            item.AddSymbol(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.AddSymbol(compiler);
        }
        foreach (IItem item in items) {
            item.SetSemantic(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.SetSemantic(compiler);
        }
    }
}
