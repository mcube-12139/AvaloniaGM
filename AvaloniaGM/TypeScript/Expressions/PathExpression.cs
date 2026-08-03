using AvaloniaGM.TypeScript;
using AvaloniaGM.TypeScript.Symbols;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class PathExpression(TextPosition position, string[] segments) : IExpression {
        readonly TextPosition position = position;
        readonly string[] segments = segments;

        void IExpression.Call(Generator generator) {
            IValueSymbol symbol = generator.GetSymbol(segments[0], position).AsValue(position, generator);
            symbol.Call(position, generator);
        }

        void IExpression.Evaluate(Generator generator) {
            IValueSymbol symbol = generator.GetSymbol(segments[0], position).AsValue(position, generator);
            symbol.Load(position, generator);
        }
    }
}
