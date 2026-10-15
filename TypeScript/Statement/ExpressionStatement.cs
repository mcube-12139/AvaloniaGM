using TypeScript.Expression;

namespace TypeScript.Statement;

public class ExpressionStatement(IExpression expression): IStatement {
    public readonly IExpression expression = expression;

    void IStatement.SetSemantic(Compiler compiler) {
        expression.SetSemantic(null, compiler);
    }
}
