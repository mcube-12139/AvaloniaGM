using TypeScript.Expression;
using TypeScript.FlowContext;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Statement;

public class IfStatement(SourcePosition position, IExpression condition, BlockStatement body, IStatement? elseBody) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IExpression condition = condition;
    public readonly BlockStatement body = body;
    public readonly IStatement? elseBody = elseBody;

    SourcePosition IStatement.GetPosition() => position;

    void IStatement.SetSemantic(Compiler compiler) {
        condition.SetSemantic(null, compiler);

        compiler.EnterFlow(new IfFlowContext());
        body.SetSemantic(compiler);

        if (elseBody != null) {
            compiler.StartElse();
            elseBody.SetSemantic(compiler);
        }

        compiler.LeaveFlow();
    }

    void IStatement.Generate(StatementContext context) {
        ExpressionHelp.Generate(condition, context);
        var jumpFalse = JumpFalseInstruction.Unresolved();
        context.AddInstruction(jumpFalse);

        ((IStatement)body).Generate(context);
        if (elseBody != null) {
            var jump = JumpInstruction.Unresolved();
            context.AddInstruction(jump);
            jumpFalse.index = context.GetInstructionCount();

            elseBody.Generate(context);

            jump.index = context.GetInstructionCount();
        } else {
            jumpFalse.index = context.GetInstructionCount();
        }
    }
}
