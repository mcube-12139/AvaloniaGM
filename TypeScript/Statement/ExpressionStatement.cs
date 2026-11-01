using TypeScript.Expression;
using TypeScript.Item;

namespace TypeScript.Statement;

public class ExpressionStatement(IExpression expression): IStatement {
    public readonly IExpression expression = expression;

    SourcePosition IStatement.GetPosition() => expression.GetPosition();

    void IItem.SetSemantic(Compiler compiler) {
        expression.SetSemantic(null, compiler);
    }
}
