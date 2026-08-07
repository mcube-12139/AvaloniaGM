using AvaloniaGM.TypeScript.Expressions;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Types {
    internal interface IType {
        string GetAppearance();
        bool IsType(IType other);
        IType GetMultiplyResultType(IType rightType, TextPosition position, Generator generator);
        IType GetDivideResultType(IType rightType, TextPosition position, Generator generator);
        IType GetModuloResultType(IType rightType, TextPosition position, Generator generator);
        IType GetAddResultType(IType rightType, TextPosition position, Generator generator);
        IType GetSubtractResultType(IType rightType, TextPosition position, Generator generator);
        IType GetLeftShiftResultType(IType rightType, TextPosition position, Generator generator);
        IType GetRightShiftResultType(IType rightType, TextPosition position, Generator generator);
        IType GetBitAndResultType(IType rightType, TextPosition position, Generator generator);
        IType GetBitXorResultType(IType rightType, TextPosition position, Generator generator);
        IType GetBitOrResultType(IType rightType, TextPosition position, Generator generator);
        IType GetGreaterResultType(IType rightType, TextPosition position, Generator generator);
        IType GetGreaterEqualResultType(IType rightType, TextPosition position, Generator generator);
        IType GetLessResultType(IType rightType, TextPosition position, Generator generator);
        IType GetLessEqualResultType(IType rightType, TextPosition position, Generator generator);
        IType GetEqualResultType(IType rightType, TextPosition position, Generator generator);
        IType GetNotEqualResultType(IType rightType, TextPosition position, Generator generator);
        IType GetCallResultType(TextPosition position, Generator generator);
        IType GetIndexResultType(IType indexType, TextPosition position, Generator generator);
        IType GetSetIndexResultType(IType rightType, IType indexType, TextPosition position, Generator generator);
        void Multiply(IExpression right, TextPosition position, Generator generator);
        void Divide(IExpression right, TextPosition position, Generator generator);
        void Modulo(IExpression right, TextPosition position, Generator generator);
        void Add(IExpression right, TextPosition position, Generator generator);
        void Subtract(IExpression right, TextPosition position, Generator generator);
        void LeftShift(IExpression right, TextPosition position, Generator generator);
        void RightShift(IExpression right, TextPosition position, Generator generator);
        void BitAnd(IExpression right, TextPosition position, Generator generator);
        void BitXor(IExpression right, TextPosition position, Generator generator);
        void BitOr(IExpression right, TextPosition position, Generator generator);
        void Greater(IExpression right, TextPosition position, Generator generator);
        void GreaterEqual(IExpression right, TextPosition position, Generator generator);
        void Less(IExpression right, TextPosition position, Generator generator);
        void LessEqual(IExpression right, TextPosition position, Generator generator);
        void Equal(IExpression right, TextPosition position, Generator generator);
        void NotEqual(IExpression right, TextPosition position, Generator generator);
        void Assign(IExpression right, TextPosition position, Generator generator);
        void AddAssign(IExpression right, TextPosition position, Generator generator);
        void SubtractAssign(IExpression right, TextPosition position, Generator generator);
        void MultiplyAssign(IExpression right, TextPosition position, Generator generator);
        void DivideAssign(IExpression right, TextPosition position, Generator generator);
        void ModuloAssign(IExpression right, TextPosition position, Generator generator);
        void BitAndAssign(IExpression right, TextPosition position, Generator generator);
        void BitOrAssign(IExpression right, TextPosition position, Generator generator);
        void BitXorAssign(IExpression right, TextPosition position, Generator generator);
        void LeftShiftAssign(IExpression right, TextPosition position, Generator generator);
        void RightShiftAssign(IExpression right, TextPosition position, Generator generator);
        void GetIndex(UndertaleVariable variable, IExpression index, TextPosition position, Generator generator);
        void SetIndex(UndertaleVariable variable, IExpression right, IExpression index, TextPosition position, Generator generator);
    }
}
