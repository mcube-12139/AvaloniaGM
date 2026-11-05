using TypeScript.ExceptionSpace;
using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Expression;

public interface IExpression {
    internal ExpressionHead GetHead();
    SourcePosition GetPosition() => GetHead().position;
    IType GetTargetType() => GetHead().targetType ?? GetResultType();
    void SetTargetType(IType target) {
        IType type = GetResultType();
        if (!TypeHelp.IsType(type, target)) {
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, [type.GetAppearance(), target.GetAppearance()], GetPosition());
        }
        GetHead().targetType = target;
    }

    IType GetResultType();
    internal void SetSemantic(IType? target, Compiler compiler);
    internal IPlaceExpression AsPlace(Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_PLACE, [], GetPosition());
    }
    internal void Generate(StatementContext context) => throw new NotImplementedException();
    internal void GenerateCall(CallExpression call, StatementContext context) => throw new NotImplementedException();
}

