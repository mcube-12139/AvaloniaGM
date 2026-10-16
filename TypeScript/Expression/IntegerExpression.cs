using TypeScript.Type;

namespace TypeScript.Expression;

public class IntegerExpression(TextPosition position, string valueStr) : IExpression {
    public readonly TextPosition position = position;
    public readonly string valueStr = valueStr;
    IType? type;

    TextPosition IExpression.GetPosition() => position;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        /*
        if (type == null) {
            if (valueStr.Length < 5 || (valueStr.Length == 5 && string.Compare(valueStr, "32768") < 0)) {
                type = PrimitiveType.INTEGER;
            } else if (valueStr.Length < 10 || (valueStr.Length == 10 && string.Compare(valueStr, "2147483648") < 0)) {
                type = PrimitiveType.INTEGER;
            } else if (valueStr.Length < 19 || (valueStr.Length == 19 && string.Compare(valueStr, "9223372036854775808") < 0)) {
                type = PrimitiveType.LONG;
            } else {
                throw compiler.SemanticError(SemanticErrorType.TOO_LARGE_INTEGER, [valueStr], position);
            }
        } else {
        }
        */
        type = PrimitiveType.INTEGER;
    }

    IType IExpression.GetResultType() => type!;
}
