using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public class ArrayExpression(TextPosition position, IExpression[] elements) : IExpression {
    public readonly TextPosition position = position;
    public readonly IExpression[] elements = elements;

    IType? type;

    TextPosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        if (elements.Length != 0) {
            elements[0].SetSemantic(null, compiler);
            IType firstType = elements[0].GetResultType();
            for (int i = 1; i != elements.Length; ++i) {
                IExpression expression = elements[i];
                expression.SetSemantic(null, compiler);
                IType elementType = expression.GetResultType();
                if (!TypeHelp.IsType(elementType, firstType)) {
                    throw compiler.SemanticError(SemanticErrorType.WRONG_TYPE, [elementType.GetAppearance(), firstType.GetAppearance()], expression.GetPosition());
                }
            }

            type = new ArrayType(firstType);
        } else {
            if (target == null) {
                throw compiler.SemanticError(SemanticErrorType.UNKNOWN_TYPE, [], position);
            }
            if (target is not ArrayType) {
                throw compiler.SemanticError(SemanticErrorType.WRONG_TYPE, ["[]", target.GetAppearance()], position);
            }
            type = target;
        }
    }
}
