using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Statement;

public class ReturnStatement(SourcePosition position, IExpression? value) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IExpression? value = value;

    IType? targetType;

    SourcePosition IStatement.GetPosition() => position;

    public IType GetTargetType() => targetType!;

    void IStatement.SetSemantic(Compiler compiler) {
        targetType = compiler.GetResultType();

        value?.SetSemantic(targetType, compiler);
        IType type = value?.GetResultType() ?? TupleType.EMPTY;
        if (!TypeHelp.IsType(type, targetType)) {
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, [type.GetAppearance(), targetType.GetAppearance()], position);
        }

        compiler.SetReturned();
    }
}
