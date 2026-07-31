using AvaloniaGM.Entities.Statement;
using AvaloniaGM.Services;

namespace AvaloniaGM.Entities {
    internal class CodeRoot(IStatement[] statements) {
        readonly IStatement[] statements = statements;

        internal void Generate(CodeGenerator generator) {
            foreach (IStatement statement in statements) {
                statement.Execute(generator);
            }
        }
    }
}
