using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Statement;

public class ContinueStatement(SourcePosition position, string? label) : IStatement {
    public readonly SourcePosition position = position;
    public readonly string? label = label;

    ILoopStatement? loop;

    SourcePosition IStatement.GetPosition() => position;

    void IStatement.SetSemantic(Compiler compiler) {
        loop = compiler.GetLoop(label, position);
    }

    void IStatement.Generate(StatementContext context) {
        context.AddInstructionWithPosition(new JumpInstruction(loop!.GetStart()), position);
    }
}
