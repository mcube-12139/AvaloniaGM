namespace AvaloniaGM.Entities.Tokens {
    internal class IdentifierToken(string name): IToken {
        internal string name = name;

        string IToken.GetAppearance() {
            return name;
        }
    }
}
