using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Symbols {
    internal interface IValueSymbol: ISymbol {
        void Call(TextPosition position, TypeScriptGenerator generator);
        void Load(TextPosition position, TypeScriptGenerator generator);
    }
}
