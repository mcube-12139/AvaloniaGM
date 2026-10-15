namespace TypeScript.Statement;

internal class BlockStatement(TextPosition position, IStatement[] statements) : IStatement {
    internal void ExecuteWithStart(Compiler compiler, Action? start) {
        compiler.EnterNameSpace();
        start?.Invoke();

        foreach (IStatement statement in statements) {
            statement.SetSemantic(compiler);
        }

        compiler.LeaveNameSpace();
    }

    public void SetSemantic(Compiler compiler) {
        ExecuteWithStart(compiler, null);
    }
}
