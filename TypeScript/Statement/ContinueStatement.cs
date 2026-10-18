namespace TypeScript.Statement;

public class ContinueStatement(TextPosition position, string? label) : IStatement {
    public readonly TextPosition position = position;
    public readonly string? label = label;

    ILoopStatement? loop;

    public ILoopStatement GetLoop() => loop!;

    void IStatement.SetSemantic(Compiler compiler) {
        loop = compiler.GetLoop(label, position);
    }
}
