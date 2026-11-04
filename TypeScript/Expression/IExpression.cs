using TypeScript.ExceptionSpace;
using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Expression;

public interface IExpression {
    SourcePosition GetPosition();
    IType GetResultType();
    internal void SetSemantic(IType? target, Compiler compiler);
    internal IPlaceExpression AsPlace(Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_PLACE, [], GetPosition());
    }
    internal void Generate(StatementContext context) => throw new NotImplementedException();
    internal void GenerateCall(CallExpression call, StatementContext context) => throw new NotImplementedException();
}
