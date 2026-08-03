using System.Collections.Generic;

namespace AvaloniaGM.TypeScript.Tokens {
    internal class FixedToken(string appearance): IToken {
        internal static FixedToken LET = new("let");

        internal static FixedToken END = new("[End]");
        internal static FixedToken LEFT_PARENTHESIS = new("(");
        internal static FixedToken RIGHT_PARENTHESIS = new(")");
        internal static FixedToken LEFT_BRACE = new("{");
        internal static FixedToken RIGHT_BRACE = new("}");
        internal static FixedToken SEMICOLON = new(";");
        internal static FixedToken COLON = new(":");
        internal static FixedToken COMMA = new(",");
        internal static FixedToken EQUAL = new("=");

        readonly static Dictionary<string, FixedToken> keywords = new() {
            { "let", LET }
        };

        internal static FixedToken? GetKeyword(string name) {
            return keywords.GetValueOrDefault(name);
        }

        string IToken.GetAppearance() {
            return appearance;
        }
    }
}
