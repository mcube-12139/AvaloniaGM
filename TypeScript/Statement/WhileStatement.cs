using TypeScript.Expression;
using TypeScript.FlowContext;

namespace TypeScript.Statement;

public class WhileStatement(SourcePosition position, string? label, IExpression condition, BlockStatement body) : ILoopStatement {
    public readonly SourcePosition position = position;
    public readonly string? label = label;
    public readonly IExpression condition = condition;
    public readonly BlockStatement body = body;

    void IStatement.SetSemantic(Compiler compiler) {
        condition.SetSemantic(null, compiler);

        compiler.EnterFlow(new WhileFlowContext());
        body.SetSemanticWithStart(compiler, () => {
            compiler.EnterLoop(label, this);
        });
        compiler.LeaveLoop();
        compiler.LeaveFlow();
    }
}
