using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class IntegerExpression(TextPosition position, string valueStr) : IExpression {
        IType? type;
        int byteLength;

        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            GetResultType(generator);
            if (byteLength == 2) {
                generator.PushInt16(short.Parse(valueStr));
            } else if (byteLength == 4) {
                generator.PushInt32(int.Parse(valueStr));
            } else if (byteLength == 8) {
                generator.PushInt64(long.Parse(valueStr));
            } else {
                throw new System.Exception("wtf");
            }
        }

        public IType GetResultType(Generator generator) {
            if (type == null) {
                if (valueStr.Length < 5 || (valueStr.Length == 5 && string.Compare(valueStr, "32768") < 0)) {
                    type = PrimitiveType.INTEGER;
                    byteLength = 2;
                } else if (valueStr.Length < 10 || (valueStr.Length == 10 && string.Compare(valueStr, "2147483648") < 0)) {
                    type = PrimitiveType.INTEGER;
                    byteLength = 4;
                } else if (valueStr.Length < 19 || (valueStr.Length == 19 && string.Compare(valueStr, "9223372036854775808") < 0)) {
                    type = PrimitiveType.LONG;
                    byteLength = 8;
                } else {
                    throw generator.SemanticError(Exceptions.SemanticErrorType.TOO_LARGE_INTEGER, [valueStr], position);
                }
            }

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
