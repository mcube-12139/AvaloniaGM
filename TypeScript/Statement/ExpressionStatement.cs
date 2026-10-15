using TypeScript.Expression;

namespace TypeScript.Statement;

internal class ExpressionStatement(IExpression expression): IStatement {
    readonly IExpression expression = expression;

    void IStatement.SetSemantic(Compiler compiler) {
        expression.SetSemantic(null, compiler);
    }
}
