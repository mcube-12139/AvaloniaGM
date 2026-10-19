using TypeScript.Item;

namespace TypeScript.Statement;

public class BlockStatement(SourcePosition position, IItem[] items, IStatement[] statements) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IItem[] items = items;
    public readonly IStatement[] statements = statements;

    internal void SetSemanticWithStart(Compiler compiler, Action? start) {
        compiler.EnterNewNameSpace();
        start?.Invoke();

        foreach (IItem item in items) {
            item.AddSymbol(compiler);
        }
        foreach (IItem item in items) {
            item.SetSemantic(compiler);
        }
        foreach (IStatement statement in statements) {
            statement.SetSemantic(compiler);
        }

        compiler.LeaveNameSpace();
    }

    internal void SetSemantic(Compiler compiler) {
        SetSemanticWithStart(compiler, null);
    }

    void IStatement.SetSemantic(Compiler compiler) => SetSemantic(compiler);
}
