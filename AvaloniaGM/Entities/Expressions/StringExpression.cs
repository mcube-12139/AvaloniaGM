using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Expressions {
    internal class StringExpression(TextPosition position, string value) : IExpression {
        readonly TextPosition position = position;
        readonly string value = value;

        void IExpression.Call(TypeScriptGenerator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(TypeScriptGenerator generator) {
            generator.PushString(value);
            generator.PushType(UndertaleModLib.Models.UndertaleInstruction.DataType.String);
        }
    }
}
