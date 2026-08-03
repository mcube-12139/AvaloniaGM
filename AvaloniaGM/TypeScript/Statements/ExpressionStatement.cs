using AvaloniaGM.TypeScript;
using AvaloniaGM.TypeScript.Expressions;

namespace AvaloniaGM.TypeScript.Statements {
    internal class ExpressionStatement(IExpression expression): IStatement {
        readonly IExpression expression = expression;

        void IStatement.Execute(Generator generator) {
            expression.Evaluate(generator);
            generator.PopUnused();
        }
    }
}
