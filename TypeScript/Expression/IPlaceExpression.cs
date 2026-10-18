namespace TypeScript.Expression;

public interface IPlaceExpression : IExpression {
    IPlaceExpression IExpression.AsPlace(Compiler compiler) {
        return this;
    }
}
