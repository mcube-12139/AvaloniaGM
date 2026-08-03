using AvaloniaGM.TypeScript;

namespace AvaloniaGM.TypeScript.Symbols {
    internal interface ISymbol {
        IValueSymbol AsValue(TextPosition position, Generator generator);
    }
}
