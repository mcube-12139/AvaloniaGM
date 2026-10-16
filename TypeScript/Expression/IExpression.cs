using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public interface IExpression {
    TextPosition GetPosition();
    IType GetResultType();
    internal void SetSemantic(IType? target, Compiler compiler);
    internal IPlaceExpression AsPlace(Compiler compiler) {
        throw compiler.SemanticError(SemanticErrorType.NOT_PLACE, [], GetPosition());
    }
}
