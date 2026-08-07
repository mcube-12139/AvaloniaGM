using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

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

        void IPlaceExpression.AddAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.AddAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.SubtractAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.SubtractAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.MultiplyAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.MultiplyAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.DivideAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.DivideAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.ModuloAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.ModuloAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.BitAndAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.BitAndAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.BitOrAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.BitOrAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.BitXorAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.BitXorAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.LeftShiftAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.LeftShiftAssign(right, position, generator);

            SetSymbol(generator);
            symbol!.AsValue(position, generator).Store(position, generator);
        }

        void IPlaceExpression.RightShiftAssign(IExpression right, Generator generator) {
            Evaluate(generator);
            GetResultType(generator);
            type!.RightShiftAssign(right, position, generator);

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

        public void Evaluate(Generator generator) {
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

        void IExpression.GetIndex(IExpression index, Generator generator) {
            SetSymbol(generator);
            symbol!.AsValue(position, generator).LoadIndex(index, position, generator);
        }

        void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
            SetSymbol(generator);
            symbol!.AsValue(position, generator).StoreIndex(right, index, position, generator);
        }

        UndertaleVariable IExpression.GetIndexDuplicate(IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
