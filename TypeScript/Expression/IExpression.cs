using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public interface IExpression {
    TextPosition GetPosition();
    IPlaceExpression AsPlace(Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.NOT_PLACE, [], GetPosition());
    }
    IType GetResultType();
    void SetSemantic(IType? target, Compiler compiler);
}
