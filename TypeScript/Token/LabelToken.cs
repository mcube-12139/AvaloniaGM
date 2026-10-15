namespace TypeScript.Token {
    internal class LabelToken(string name): IToken {
        internal string name = name;

        string IToken.GetAppearance() {
            return $"l'{name}";
        }
    }
}
