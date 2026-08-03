namespace AvaloniaGM.TypeScript.Statements {
    internal class BlockStatement(TextPosition position, IStatement[] statements) : IStatement {
        void IStatement.Execute(Generator generator) {
            generator.EnterBlock();

            foreach (IStatement statement in statements) {
                statement.Execute(generator);
            }

            generator.LeaveBlock();
        }
    }
}
