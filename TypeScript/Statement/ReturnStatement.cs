using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Statement;

public class ReturnStatement(SourcePosition position, IExpression? value) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IExpression? value = value;

    void IStatement.SetSemantic(Compiler compiler) {
        IType resultType = compiler.GetResultType();

        value?.SetSemantic(resultType, compiler);
        IType type = value?.GetResultType() ?? TupleType.EMPTY;
        if (!TypeHelp.IsType(type, resultType)) {
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, [type.GetAppearance(), resultType.GetAppearance()], position);
        }

        compiler.SetReturned();
    }
}
