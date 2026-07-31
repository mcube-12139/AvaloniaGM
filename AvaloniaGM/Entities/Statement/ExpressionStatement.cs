using AvaloniaGM.Entities.Expressions;
using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Statement {
    internal class ExpressionStatement(IExpression expression): IStatement {
        readonly IExpression expression = expression;

        void IStatement.Execute(CodeGenerator generator) {
            expression.Evaluate(generator);
            generator.PopUnused();
        }
    }
}
