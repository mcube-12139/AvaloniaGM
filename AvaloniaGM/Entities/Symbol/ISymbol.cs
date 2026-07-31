using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Symbol {
    internal interface ISymbol {
        IValueSymbol AsValue(TextPosition position, CodeGenerator generator);
    }
}
