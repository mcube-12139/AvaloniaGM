using TypeScript.Expression;

namespace TypeScript.Statement;

public class ExpressionStatement(IExpression expression): IStatement {
    public readonly IExpression expression = expression;

    SourcePosition IStatement.GetPosition() => expression.GetPosition();

    void IStatement.SetSemantic(Compiler compiler) {
        expression.SetSemantic(null, compiler);
    }
}
