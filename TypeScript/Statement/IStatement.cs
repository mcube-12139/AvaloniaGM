using TypeScript.GenerateContext;

namespace TypeScript.Statement;

public interface IStatement {
    SourcePosition GetPosition();
    internal void SetSemantic(Compiler compiler);
    internal void Generate(StatementContext context) => throw new NotImplementedException();
}
