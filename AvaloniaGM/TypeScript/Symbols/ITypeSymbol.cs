using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Symbols {
    internal interface ITypeSymbol: ISymbol {
        IType GetSharkType(TextPosition position, Generator generator);
    }
}
