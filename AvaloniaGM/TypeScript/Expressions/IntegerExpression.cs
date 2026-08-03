using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class IntegerExpression(TextPosition position, string valueStr) : IExpression {
        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            if (valueStr.Length < 5 || (valueStr.Length == 5 && string.Compare(valueStr, "32768") < 0)) {
                generator.PushInt16(short.Parse(valueStr));
                generator.PushType(UndertaleInstruction.DataType.Int32);
            } else if (valueStr.Length < 10 || (valueStr.Length == 10 && string.Compare(valueStr, "2147483648") < 0)) {
                generator.PushInt32(int.Parse(valueStr));
                generator.PushType(UndertaleInstruction.DataType.Int32);
            } else if (valueStr.Length < 19 || (valueStr.Length == 19 && string.Compare(valueStr, "9223372036854775808") < 0)) {
                generator.PushInt64(long.Parse(valueStr));
                generator.PushType(UndertaleInstruction.DataType.Int64);
            } else {
                throw generator.SemanticError(Exceptions.SemanticErrorType.TOO_LARGE_INTEGER, [valueStr], position);
            }
        }
    }
}
