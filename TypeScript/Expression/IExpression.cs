using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Expression;

public interface IExpression {
    internal ExpressionHead GetHead();
    SourcePosition GetPosition() => GetHead().position;
    IType? GetTargetType() => GetHead().targetType;
    internal void SetTargetType(IType targetType) {
        if (!TypeHelp.IsType(GetResultType(), targetType)) {
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, [GetResultType().GetAppearance(), targetType.GetAppearance()], GetPosition());
        }

        GetHead().targetType = targetType;
    }

    IType GetResultType();
    internal FunctionType SetCallSemantic(IType? target, IExpression[] parameters, Compiler compiler) {
        SetSemantic(null, compiler);
        FunctionType calleeType = GetResultType().AsFunctionType(GetPosition(), compiler);
        calleeType.EnsureCanCall(parameters, GetPosition(), compiler);
        return calleeType;
    }
    internal void SetSemantic(IType? target, Compiler compiler);
    internal IPlaceExpression AsPlace(Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_PLACE, [], GetPosition());
    }
}

