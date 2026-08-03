namespace AvaloniaGM.TypeScript.Expressions {
    internal class BinaryExpression(TextPosition position, IExpression left, IExpression right, BinaryOperator op): IExpression {
        void IExpression.Evaluate(Generator generator) {
            op.evaluate(left, right, position, generator);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
