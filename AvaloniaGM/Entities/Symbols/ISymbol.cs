using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Symbols {
    internal interface ISymbol {
        IValueSymbol AsValue(TextPosition position, TypeScriptGenerator generator);
    }
}
