namespace AvaloniaGM.Entities.Tokens {
    internal class StringToken(string value): IToken {
        internal string value = value;

        string IToken.GetAppearance() {
            return $"\"{value}\"";
        }
    }
}
