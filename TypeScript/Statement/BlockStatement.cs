using TypeScript.Item;

namespace TypeScript.Statement;

public class BlockStatement(SourcePosition position, IItem[] items, ImplNode[] impls, IStatement[] statements) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IItem[] items = items;
    public readonly ImplNode[] impls = impls;
    public readonly IStatement[] statements = statements;

    NameSpace? space;

    SourcePosition IStatement.GetPosition() => position;

    void EnterNameSpace(Compiler compiler) {
        if (space == null) {
            space = compiler.EnterNewNameSpace();
        } else {
            compiler.space = space;
        }
    }

    void IItem.AddItemSymbol(Compiler compiler) {
        EnterNameSpace(compiler);

        foreach (IItem item in items) {
            item.AddItemSymbol(compiler);
        }
        foreach (IStatement statement in statements) {
            statement.AddItemSymbol(compiler);
        }

        compiler.LeaveNameSpace();
    }

    void IItem.AddImplSymbol(Compiler compiler) {
        EnterNameSpace(compiler);

        foreach (IItem item in items) {
            item.AddImplSymbol(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.AddSymbol(compiler);
        }
        foreach (IStatement statement in statements) {
            statement.AddImplSymbol(compiler);
        }

        compiler.LeaveNameSpace();
    }

    internal void SetSemanticWithStart(Compiler compiler, Action? start) {
        EnterNameSpace(compiler);
        start?.Invoke();

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

    void IItem.SetSemantic(Compiler compiler) => SetSemantic(compiler);
}
