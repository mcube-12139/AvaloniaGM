namespace TypeScript.Token {
    internal class IntegerToken(string valueStr) : IToken {
        internal string valueStr = valueStr;

        string IToken.GetAppearance() {
            return valueStr;
        }
    }
}
