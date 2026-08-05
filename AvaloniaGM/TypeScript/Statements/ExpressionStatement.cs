using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Statements {
    internal class ExpressionStatement(IExpression expression): IStatement {
        readonly IExpression expression = expression;

        void IStatement.Execute(Generator generator) {
            expression.Evaluate(generator);
            if (expression.GetResultType(generator) != TupleType.EMPTY) {
                generator.PopUnused();
            }
        }
    }
}
