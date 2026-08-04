using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Expressions {
    internal interface IExpression {
        IPlaceExpression AsPlace(Generator generator);
        IType GetResultType(Generator generator);
        void Evaluate(Generator generator);
        void Call(Generator generator);
    }
}
