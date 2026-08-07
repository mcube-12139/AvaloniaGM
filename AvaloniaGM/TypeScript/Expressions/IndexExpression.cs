using AvaloniaGM.TypeScript.Types;
using System;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class IndexExpression(TextPosition position, IExpression indexed, IExpression index) : IPlaceExpression {
        void IPlaceExpression.AddAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        IPlaceExpression IExpression.AsPlace(Generator generator) => this;

        void IPlaceExpression.Assign(IExpression right, Generator generator) {
            indexed.SetIndex(right, index, generator);
        }

        void IPlaceExpression.BitAndAssign(IExpression right, Generator generator) {
            UndertaleVariable variable = indexed.GetIndexDuplicate(index, generator);
            indexed.GetResultType(generator).BitAndAssign(right, position, generator);
            generator.Store(variable, UndertaleInstruction.VariableType.Array);
        }

        void IPlaceExpression.BitOrAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IPlaceExpression.BitXorAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IExpression.Call(Generator generator) {
            throw new NotImplementedException();
        }

        void IPlaceExpression.DivideAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            indexed.GetIndex(index, generator);
        }

        void IExpression.GetIndex(IExpression index, Generator generator) {
            throw new NotImplementedException();
        }

        UndertaleVariable IExpression.GetIndexDuplicate(IExpression index, Generator generator) {
            throw new NotImplementedException();
        }

        IType IExpression.GetResultType(Generator generator) {
            IType indexType = index.GetResultType(generator);
            return indexed.GetResultType(generator).GetIndexResultType(indexType, position, generator);
        }

        void IPlaceExpression.LeftShiftAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IPlaceExpression.ModuloAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IPlaceExpression.MultiplyAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IPlaceExpression.RightShiftAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }

        void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
            throw new NotImplementedException();
        }

        void IPlaceExpression.SubtractAssign(IExpression right, Generator generator) {
            throw new NotImplementedException();
        }
    }
}
