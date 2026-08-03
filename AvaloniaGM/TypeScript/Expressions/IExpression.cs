using AvaloniaGM.TypeScript;

namespace AvaloniaGM.TypeScript.Expressions {
    internal interface IExpression {
        void Evaluate(Generator generator);
        void Call(Generator generator);
    }
}
