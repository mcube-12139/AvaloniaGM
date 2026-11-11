using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Expression;

internal class ExpressionHelp {
    internal static void Generate(IExpression expression, StatementContext context) {
        expression.Generate(context);
        IType targetType = expression.GetTargetType();
        IType resultType = expression.GetResultType();
        if (resultType != targetType) {
            // 隐式向上转型
            context.AddInstruction(UpcastInstruction.Instantiate(resultType.ToIntermediate(), targetType.ToIntermediate()));
        }
    }
}
