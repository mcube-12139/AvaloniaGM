namespace AvaloniaGM.TypeScript.Symbols {
    internal interface ISymbol {
        IValueSymbol AsValue(TextPosition position, Generator generator);
        ITypeSymbol AsType(TextPosition position, Generator generator);
    }
}
