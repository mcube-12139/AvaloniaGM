using AvaloniaGM.TypeScript;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class StringExpression(TextPosition position, string value) : IExpression {
        readonly TextPosition position = position;
        readonly string value = value;

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            generator.PushString(value);
            generator.PushType(UndertaleModLib.Models.UndertaleInstruction.DataType.String);
        }
    }
}
