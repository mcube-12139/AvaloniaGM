namespace TypeScript.Statement;

public interface IStatement {
    SourcePosition GetPosition();
    internal void SetSemantic(Compiler compiler);
}
