using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Statement;

public class BreakStatement(SourcePosition position, string? label) : IStatement {
    public readonly SourcePosition position = position;
    public readonly string? label = label;

    ILoopStatement? loop;

    SourcePosition IStatement.GetPosition() => position;

    void IStatement.SetSemantic(Compiler compiler) {
        loop = compiler.GetLoop(label, position);
    }

    void IStatement.Generate(StatementContext context) {
        var jump = JumpInstruction.Unresolved();
        context.AddInstructionWithPosition(jump, position);
        loop!.PromiseBreak(jump);
    }
}
