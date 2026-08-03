using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Expressions {
    internal interface IExpression {
        void Evaluate(TypeScriptGenerator generator);
        void Call(TypeScriptGenerator generator);
    }
}
