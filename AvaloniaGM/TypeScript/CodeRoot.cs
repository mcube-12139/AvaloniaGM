using AvaloniaGM.TypeScript.Statements;

namespace AvaloniaGM.TypeScript {
    internal class CodeRoot(IStatement[] statements) {
        readonly IStatement[] statements = statements;

        internal void Generate(Generator generator) {
            foreach (IStatement statement in statements) {
                statement.Execute(generator);
            }
        }
    }
}
