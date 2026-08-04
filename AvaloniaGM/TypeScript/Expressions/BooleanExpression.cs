using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class BooleanExpression(TextPosition position, bool value) : IExpression {
        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }

        void IExpression.Call(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_CALLABLE, [value ? "true" : "false"], position);
        }

        void IExpression.Evaluate(Generator generator) {
            generator.PushBoolean(value);
        }

        public IType GetResultType(Generator generator) {
            return PrimitiveType.BOOLEAN;
        }
    }
}
