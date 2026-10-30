using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Type;

namespace TypeScript.Statement;

public class ReturnStatement(SourcePosition position, IExpression? value) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IExpression? value = value;

    SourcePosition IStatement.GetPosition() => position;

    void IStatement.SetSemantic(Compiler compiler) {
        IType? targetType = compiler.GetResultType();

        if (value != null) {
            value.SetSemantic(targetType, compiler);
            value.SetTargetType(targetType);
        } else {
            // 只有返回类型是()可以没有返回式
            if (targetType != TupleType.EMPTY) {
                throw new SemanticException(SemanticErrorType.MISSING_RESULT_EXPRESSION, [], position);
            }
        }

        compiler.SetReturned();
    }
}
