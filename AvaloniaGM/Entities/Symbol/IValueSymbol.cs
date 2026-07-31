using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Symbol {
    internal interface IValueSymbol: ISymbol {
        void Call(TextPosition position, CodeGenerator generator);
    }
}
