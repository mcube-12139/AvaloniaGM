using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Expressions {
    internal interface IExpression {
        void Evaluate(CodeGenerator generator);
        void Call(CodeGenerator generator);
    }
}
