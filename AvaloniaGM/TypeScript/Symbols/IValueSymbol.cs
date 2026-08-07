using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Symbols {
    internal interface IValueSymbol: ISymbol {
        IType GetValueType(TextPosition position, Generator generator);
        void Call(TextPosition position, Generator generator);
        void Load(TextPosition position, Generator generator);
        void Store(TextPosition position, Generator generator);
        void LoadIndex(IExpression index, TextPosition position, Generator generator);
        void StoreIndex(IExpression right, IExpression index, TextPosition position, Generator generator);
    }
}
