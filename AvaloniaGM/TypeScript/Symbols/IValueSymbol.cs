using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Symbols {
    internal interface IValueSymbol: ISymbol {
        IType GetValueType(TextPosition position, Generator generator);
        void Call(TextPosition position, Generator generator);
        void Load(TextPosition position, Generator generator);
    }
}
