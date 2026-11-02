using TypeScript.Item;
using TypeScript.NameSpace;

namespace TypeScript.Statement;

public class BlockStatement(SourcePosition position, IItem[] items, ImplNode[] impls, IStatement[] statements) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IItem[] items = items;
    public readonly ImplNode[] impls = impls;
    public readonly IStatement[] statements = statements;

    BlockSpace? space;

    SourcePosition IStatement.GetPosition() => position;

    void IItem.AddItemSymbol(Compiler compiler) {
        space = compiler.EnterOrNewBlockSpace(space);

        foreach (IItem item in items) {
            item.AddItemSymbol(compiler);
        }
        foreach (IStatement statement in statements) {
            statement.AddItemSymbol(compiler);
        }

        compiler.LeaveBlockSpace();
    }

    void IItem.AddImpl(Compiler compiler) {
        space = compiler.EnterOrNewBlockSpace(space);

        foreach (IItem item in items) {
            item.AddImpl(compiler);
        }
        foreach (ImplNode impl in impls) {
            impl.AddImpl(compiler);
        }
        foreach (IStatement statement in statements) {
            statement.AddImpl(compiler);
        }

        compiler.LeaveBlockSpace();
    }

    internal void SetSemanticWithStart(Compiler compiler, Action? start) {
        space = compiler.EnterOrNewBlockSpace(space);
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

        compiler.LeaveBlockSpace();
    }

    internal void SetSemantic(Compiler compiler) {
        SetSemanticWithStart(compiler, null);
    }

    void IItem.SetSemantic(Compiler compiler) => SetSemantic(compiler);
}
