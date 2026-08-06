using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Statements {
    internal class ReturnStatement(TextPosition position, IExpression? expression): IStatement {
        void IStatement.Execute(Generator generator) {
            // 检查结果类型
            IType type = expression?.GetResultType(generator) ?? TupleType.EMPTY;
            IType requiredType = generator.GetResultType();
            if (!type.IsType(requiredType)) {
                throw generator.SemanticError(SemanticErrorType.WRONG_TYPE, [type.GetAppearance(), requiredType.GetAppearance()], position);
            }

            if (expression != null) {
                expression.Evaluate(generator);
                if (expression.GetResultType(generator) != TupleType.EMPTY) {
                    generator.Return();
                } else {
                    generator.Exit();
                }
            } else {
                generator.Exit();
            }
        }
    }
}
