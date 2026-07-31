namespace AvaloniaGM.Entities.Token {
    internal class FixedToken: IToken {
        internal static FixedToken END = new();
        internal static FixedToken LEFT_PARENTHESIS = new();
        internal static FixedToken RIGHT_PARENTHESIS = new();
        internal static FixedToken SEMICOLON = new();
        internal static FixedToken COMMA = new();
    }
}
