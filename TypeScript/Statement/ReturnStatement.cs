using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Statement;

public class ReturnStatement(TextPosition position, IExpression? value) : IStatement {
    public readonly TextPosition position = position;
    public readonly IExpression? value = value;

    void IStatement.SetSemantic(Compiler compiler) {
        IType resultType = compiler.GetResultType();

        value?.SetSemantic(resultType, compiler);
        IType type = value?.GetResultType() ?? TupleType.EMPTY;
        if (!TypeHelp.IsType(type, resultType)) {
            throw compiler.SemanticError(ExceptionSpace.SemanticErrorType.WRONG_TYPE, [type.GetAppearance(), resultType.GetAppearance()], position);
        }
    }
}
