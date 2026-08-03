using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Expressions {
    internal class CallExpression(TextPosition position, IExpression called, IExpression[] parameters): IExpression {
        readonly TextPosition position = position;
        readonly IExpression called = called;
        readonly IExpression[] parameters = parameters;

        void IExpression.Evaluate(TypeScriptGenerator generator) {
            for (int i = parameters.Length - 1; i != -1; --i) {
                parameters[i].Evaluate(generator);
                generator.Convert(UndertaleModLib.Models.UndertaleInstruction.DataType.Variable);
            }
            called.Call(generator);
            generator.PushType(UndertaleModLib.Models.UndertaleInstruction.DataType.Variable);
        }

        void IExpression.Call(TypeScriptGenerator generator) {
            throw new System.NotImplementedException();
        }
    }
}
