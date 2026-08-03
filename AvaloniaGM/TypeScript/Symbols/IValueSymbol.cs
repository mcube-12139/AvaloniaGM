using AvaloniaGM.TypeScript;

namespace AvaloniaGM.TypeScript.Symbols {
    internal interface IValueSymbol: ISymbol {
        void Call(TextPosition position, Generator generator);
        void Load(TextPosition position, Generator generator);
    }
}
