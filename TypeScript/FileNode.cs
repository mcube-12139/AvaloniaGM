using TypeScript.Item;

namespace TypeScript;

public class FileNode(IItem[] items) {
    public readonly IItem[] items = items;

    internal void SetSemantic(Compiler compiler) {
        foreach (IItem item in items) {
            item.AddSymbol(compiler);
        }
        foreach (IItem item in items) {
            item.SetSemantic(compiler);
        }
    }
}
