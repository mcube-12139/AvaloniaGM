using AvaloniaGM.TypeScript.Expressions;

namespace AvaloniaGM.TypeScript.Types {
    internal interface IType {
        string GetAppearance();
        IType GetCallResultType(TextPosition position, Generator generator);
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
    }
}
