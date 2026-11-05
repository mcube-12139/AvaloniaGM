using TypeScript.Expression;
using TypeScript.FlowContext;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Statement;

public class WhileStatement(SourcePosition position, string? label, IExpression condition, BlockStatement body) : ILoopStatement {
    public readonly SourcePosition position = position;
    public readonly string? label = label;
    public readonly IExpression condition = condition;
    public readonly BlockStatement body = body;

    int start;
    readonly List<JumpInstruction> breakPromises = [];

    SourcePosition IStatement.GetPosition() => position;
    int ILoopStatement.GetStart() => start;

    void IStatement.SetSemantic(Compiler compiler) {
        condition.SetSemantic(null, compiler);

        compiler.EnterFlow(new WhileFlowContext());
        body.SetSemanticWithStart(compiler, () => {
            compiler.EnterLoop(label, this);
        });
        compiler.LeaveLoop();
        compiler.LeaveFlow();
    }

    void IStatement.Generate(StatementContext context) {
        start = context.GetInstructionCount();

        ExpressionHelp.Generate(condition, context);
        var jumpFalse = JumpFalseInstruction.Unresolved();
        context.AddInstruction(jumpFalse);

        ((IStatement)body).Generate(context);
        context.AddInstruction(new JumpInstruction(start));

        int end = context.GetInstructionCount();
        jumpFalse.index = end;

        foreach (JumpInstruction jump in breakPromises) {
            jump.index = end;
        }
    }

    void ILoopStatement.PromiseBreak(JumpInstruction instruction) {
        breakPromises.Add(instruction);
    }
}
