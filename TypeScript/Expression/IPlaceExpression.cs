using TypeScript.GenerateContext;

namespace TypeScript.Expression;

public interface IPlaceExpression : IExpression {
    IPlaceExpression IExpression.AsPlace(Compiler compiler) {
        return this;
    }
    internal void SetAssignSemantic(Compiler compiler);
    internal void GenerateAssign(IExpression expression, StatementContext context) => throw new NotImplementedException();
    internal void GenerateCompoundAssignStart(StatementContext context) => throw new NotImplementedException();
    internal void GenerateCompoundAssign(StatementContext context) => throw new NotImplementedException();
}

