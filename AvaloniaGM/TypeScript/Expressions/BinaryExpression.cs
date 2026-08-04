using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class BinaryExpression(TextPosition position, IExpression left, IExpression right, BinaryOperator op): IExpression {
        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }
        
        void IExpression.Evaluate(Generator generator) {
            op.evaluate(left, right, position, generator);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        IType IExpression.GetResultType(Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
