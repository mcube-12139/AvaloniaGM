namespace AvaloniaGM.TypeScript.Tokens {
    internal class IntegerToken(string text) : IToken {
        string IToken.GetAppearance() {
            return text;
        }
    }
}
