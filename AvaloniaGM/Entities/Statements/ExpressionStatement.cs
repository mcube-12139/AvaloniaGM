using AvaloniaGM.Entities.Expressions;
using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Statements {
    internal class ExpressionStatement(IExpression expression): IStatement {
        readonly IExpression expression = expression;

        void IStatement.Execute(TypeScriptGenerator generator) {
            expression.Evaluate(generator);
            generator.PopUnused();
        }
    }
}
