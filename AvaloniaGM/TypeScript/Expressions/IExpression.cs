using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal interface IExpression {
        IPlaceExpression AsPlace(Generator generator);
        IType GetResultType(Generator generator);
        void Evaluate(Generator generator);
        void Call(Generator generator);
        void GetIndex(IExpression index, Generator generator);
        UndertaleVariable GetIndexDuplicate(IExpression index, Generator generator);
        void SetIndex(IExpression right, IExpression index, Generator generator);
    }
}
