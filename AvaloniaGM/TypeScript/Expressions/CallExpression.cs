using AvaloniaGM.TypeScript;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class CallExpression(TextPosition position, IExpression called, IExpression[] parameters): IExpression {
        readonly TextPosition position = position;
        readonly IExpression called = called;
        readonly IExpression[] parameters = parameters;

        void IExpression.Evaluate(Generator generator) {
            for (int i = parameters.Length - 1; i != -1; --i) {
                parameters[i].Evaluate(generator);
                generator.Convert(UndertaleModLib.Models.UndertaleInstruction.DataType.Variable);
            }
            called.Call(generator);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
