namespace TypeScript.Statement;

public class BlockStatement(TextPosition position, IStatement[] statements) : IStatement {
    public readonly TextPosition position = position;
    public readonly IStatement[] statements = statements;

    internal void ExecuteWithStart(Compiler compiler, Action? start) {
        compiler.EnterNameSpace();
        start?.Invoke();

        foreach (IStatement statement in statements) {
            statement.SetSemantic(compiler);
        }

        compiler.LeaveNameSpace();
    }

    internal void SetSemantic(Compiler compiler) {
        ExecuteWithStart(compiler, null);
    }

    void IStatement.SetSemantic(Compiler compiler) => SetSemantic(compiler);
}
