using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class CallExpression(TextPosition position, IExpression called, IExpression[] parameters): IExpression {
        readonly TextPosition position = position;
        readonly IExpression called = called;
        readonly IExpression[] parameters = parameters;

        IType? type;

        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }

        void IExpression.Evaluate(Generator generator) {
            for (int i = parameters.Length - 1; i != -1; --i) {
                parameters[i].Evaluate(generator);
                generator.Convert(UndertaleModLib.Models.UndertaleInstruction.DataType.Variable);
            }
            called.Call(generator);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        IType IExpression.GetResultType(Generator generator) {
            type ??= called.GetResultType(generator).GetCallResultType(position, generator);
            return type;
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
