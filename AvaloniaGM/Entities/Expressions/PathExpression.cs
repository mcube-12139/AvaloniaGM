using AvaloniaGM.Entities.Symbol;
using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Expressions {
    internal class PathExpression(TextPosition position, string[] segments) : IExpression {
        readonly TextPosition position = position;
        readonly string[] segments = segments;

        void IExpression.Call(CodeGenerator generator) {
            IValueSymbol symbol = generator.GetSymbol(segments[0]).AsValue(position, generator);
            symbol.Call(position, generator);
        }

        void IExpression.Evaluate(CodeGenerator generator) {
            throw new System.NotImplementedException();
        }
    }
}
