using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class PathExpression(TextPosition position, string[] segments) : IPlaceExpression {
        readonly TextPosition position = position;
        readonly string[] segments = segments;

        IType? type;
        ISymbol? symbol;

        IPlaceExpression IExpression.AsPlace(Generator generator) {
            return this;
        }

        void IPlaceExpression.Assign(IExpression right, Generator generator) {
            right.Evaluate(generator);

            GetResultType(generator);
            type!.Assign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
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

        public IType GetResultType(Generator generator) {
            if (type == null) {
                SetSymbol(generator);
                type = symbol!.AsValue(position, generator).GetValueType(position, generator);
            }

            return type;
        }
    }
}
