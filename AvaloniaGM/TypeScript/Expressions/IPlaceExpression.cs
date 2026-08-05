namespace AvaloniaGM.TypeScript.Expressions {
    internal interface IPlaceExpression: IExpression {
        void Assign(IExpression right, Generator generator);
        void AddAssign(IExpression right, Generator generator);
        void SubtractAssign(IExpression right, Generator generator);
        void MultiplyAssign(IExpression right, Generator generator);
        void DivideAssign(IExpression right, Generator generator);
        void ModuloAssign(IExpression right, Generator generator);
        void BitAndAssign(IExpression right, Generator generator);
        void BitOrAssign(IExpression right, Generator generator);
        void BitXorAssign(IExpression right, Generator generator);
        void LeftShiftAssign(IExpression right, Generator generator);
        void RightShiftAssign(IExpression right, Generator generator);
    }
}
