using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

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
            IType leftType = left.GetResultType(generator);
            IType rightType = right.GetResultType(generator);
            return op.getResultType(leftType, rightType, position, generator);
        }

        void IExpression.GetIndex(IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }

        UndertaleVariable IExpression.GetIndexDuplicate(IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
