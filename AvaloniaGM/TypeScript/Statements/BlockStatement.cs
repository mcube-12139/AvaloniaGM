using System;

namespace AvaloniaGM.TypeScript.Statements {
    internal class BlockStatement(TextPosition position, IStatement[] statements) : IStatement {
        internal void ExecuteWithStart(Generator generator, Action? start) {
            generator.EnterNameSpace();
            start?.Invoke();

            foreach (IStatement statement in statements) {
                statement.Execute(generator);
            }

            generator.LeaveNameSpace();
        }

        public void Execute(Generator generator) {
            ExecuteWithStart(generator, null);
        }
    }
}
