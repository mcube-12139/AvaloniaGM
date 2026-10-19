using TypeScript.Expression;
using TypeScript.FlowContext;

namespace TypeScript.Statement;

public class IfStatement(TextPosition position, IExpression condition, BlockStatement body, IStatement? elseBody) : IStatement {
    public readonly TextPosition position = position;
    public readonly IExpression condition = condition;
    public readonly BlockStatement body = body;
    public readonly IStatement? elseBody = elseBody;

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
}
