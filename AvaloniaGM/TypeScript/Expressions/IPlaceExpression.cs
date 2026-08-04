namespace AvaloniaGM.TypeScript.Expressions {
    internal interface IPlaceExpression: IExpression {
        void Assign(IExpression right, Generator generator);
    }
}
