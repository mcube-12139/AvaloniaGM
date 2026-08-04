using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class PathExpression(TextPosition position, string[] segments) : IPlaceExpression {
        readonly TextPosition position = position;
        readonly string[] segments = segments;

        ISymbol? symbol;

        IPlaceExpression IExpression.AsPlace(Generator generator) {
            return this;
        }

        void IPlaceExpression.EvaluatePlace(Generator generator) {
            // 无事可做
        }

        void SetSymbol(Generator generator) {
            symbol ??= generator.GetSymbol(segments[0], position);
        }

        void IExpression.Call(Generator generator) {
            SetSymbol(generator);
            IValueSymbol valueSymbol = symbol!.AsValue(position, generator);
            valueSymbol.Call(position, generator);
        }

        void IExpression.Evaluate(Generator generator) {
            SetSymbol(generator);
            IValueSymbol valueSymbol = symbol!.AsValue(position, generator);
            valueSymbol.Load(position, generator);
        }

        IType IExpression.GetResultType(Generator generator) {
            SetSymbol(generator);
            return symbol!.AsValue(position, generator).GetValueType(position, generator);
        }
    }
}
