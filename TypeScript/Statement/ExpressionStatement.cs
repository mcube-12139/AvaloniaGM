using TypeScript.Expression;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Statement;

public class ExpressionStatement(IExpression expression): IStatement {
    public readonly IExpression expression = expression;

    SourcePosition IStatement.GetPosition() => expression.GetPosition();

    void IStatement.SetSemantic(Compiler compiler) {
        expression.SetSemantic(null, compiler);
    }

    void IStatement.Generate(StatementContext context) {
        ExpressionHelp.Generate(expression, context);
        if (expression.GetResultType() != TupleType.EMPTY) {
            context.AddInstructionWithPosition(new PopInstruction(expression.GetResultType().ToIntermediate()), expression.GetPosition());
        }
    }
}
