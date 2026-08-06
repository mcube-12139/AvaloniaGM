using AvaloniaGM.TypeScript.Items;
using AvaloniaGM.TypeScript.Statements;

namespace AvaloniaGM.TypeScript {
    internal class CodeRoot(IItem[] items, IStatement[] statements) {
        internal void Generate(Generator generator) {
            foreach (IItem item in items) {
                item.AddSymbol(generator);
            }
            foreach (IItem item in items) {
                item.Generate(generator);
            }
            foreach (IStatement statement in statements) {
                statement.Execute(generator);
            }
        }
    }
}
