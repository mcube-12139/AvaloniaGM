using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using System.Linq;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Types {
    internal class FunctionType(IType[] parameterTypes, IType resultType) : IType {
        private readonly IType[] parameterTypes = parameterTypes;
        private readonly IType resultType = resultType;

        public string GetAppearance() {
            return $"({string.Join(", ", parameterTypes.Select(type => type.GetAppearance()))}) => {resultType.GetAppearance()}";
        }

        bool IType.IsType(IType other) {
            if (other is not FunctionType otherFun) {
                return false;
            }

            if (!resultType.IsType(otherFun.resultType)) {
                return false;
            }

            if (parameterTypes.Length != otherFun.parameterTypes.Length) {
                return false;
            }

            for (int i = 0; i != parameterTypes.Length; ++i) {
                if (!parameterTypes[i].IsType(otherFun.parameterTypes[i])) {
                    return false;
                }
            }

            return true;
        }

        IType IType.GetCallResultType(TextPosition position, Generator generator) {
            return resultType;
        }

        internal void Call(UndertaleFunction fun, Generator generator) {
            generator.Call(fun, parameterTypes.Length);
        }

        void IType.Add(IExpression other, TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} + ?"], position);
        }

        void IType.Subtract(IExpression other, TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} - ?"], position);
        }

        void IType.Multiply(IExpression other, TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} * ?"], position);
        }

        void IType.Divide(IExpression other, TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} / ?"], position);
        }

        void IType.Modulo(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.LeftShift(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.RightShift(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.BitAnd(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.BitXor(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.BitOr(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.Greater(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.GreaterEqual(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.Less(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.LessEqual(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.Equal(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.NotEqual(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.Assign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.AddAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.SubtractAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.MultiplyAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.DivideAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.ModuloAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.BitAndAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.BitOrAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.BitXorAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.LeftShiftAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.RightShiftAssign(IExpression right, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        IType IType.GetMultiplyResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetDivideResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetModuloResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetAddResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetSubtractResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetLeftShiftResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetRightShiftResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetBitAndResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetBitXorResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetBitOrResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetGreaterResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetGreaterEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetLessResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetLessEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetNotEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return this;
        }

        IType IType.GetIndexResultType(IType indexType, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        IType IType.GetSetIndexResultType(IType rightType, IType indexType, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.GetIndex(UndertaleVariable variable, IExpression index, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IType.SetIndex(UndertaleVariable variable, IExpression right, IExpression index, TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
