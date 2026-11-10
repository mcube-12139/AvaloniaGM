using TypeScript.GenerateContext;
using TypeScript.Item;

namespace TypeScript.Statement;

public class BlockStatement(SourcePosition position, IItem[] items, ImplNode[] impls, IStatement[] statements) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IItem[] items = items;
    public readonly ImplNode[] impls = impls;
    public readonly IStatement[] statements = statements;

    SourcePosition IStatement.GetPosition() => position;

    internal void SetSemanticWithStart(Compiler compiler, Action? start) {
        compiler.EnterNewNameSpace();
        start?.Invoke();

        foreach (IItem item in items) {
            item.AddItemSymbol(compiler);
        }

        foreach (IItem item in items) {
            item.AddImplSymbol(compiler);
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
        foreach (IStatement statement in statements) {
            statement.SetSemantic(compiler);
        }

        compiler.LeaveNameSpace();
    }

    internal void SetSemantic(Compiler compiler) {
        SetSemanticWithStart(compiler, null);
    }

    void IStatement.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    void IStatement.Generate(StatementContext context) {
        foreach (IItem item in items) {
            item.Generate(context);
        }
        foreach (ImplNode impl in impls) {
            impl.Generate(context);
        }
        foreach (IStatement statement in statements) {
            statement.Generate(context);
        }
    }
}
