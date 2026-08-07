using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class StringExpression(TextPosition position, string value) : IExpression {
        readonly TextPosition position = position;
        readonly string value = value;

        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            generator.PushString(value);
        }

        void IExpression.GetIndex(IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }

        UndertaleVariable IExpression.GetIndexDuplicate(IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }

        IType IExpression.GetResultType(Generator generator) {
            return PrimitiveType.STRING;
        }

        void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
