using System.Collections.Generic;

namespace AvaloniaGM.TypeScript.Tokens {
    internal class FixedToken(string appearance): IToken {
        internal static FixedToken LET = new("let");
        internal static FixedToken TYPE = new("type");
        internal static FixedToken INTERFACE = new("interface");
        internal static FixedToken AS = new("as");
        internal static FixedToken IMPLEMENT = new("implement");
        internal static FixedToken NAMESPACE = new("namespace");
        internal static FixedToken ENUM = new("enum");
        internal static FixedToken PUBLIC = new("public");
        internal static FixedToken NEVER = new("never");
        internal static FixedToken RETURN = new("return");
        internal static FixedToken YIELD = new("yield");
        internal static FixedToken FUNCTION = new("function");
        internal static FixedToken CLASS = new("class");
        internal static FixedToken IF = new("if");
        internal static FixedToken ELSE = new("else");
        internal static FixedToken FOR = new("for");
        internal static FixedToken WHILE = new("while");
        internal static FixedToken BREAK = new("break");
        internal static FixedToken CONTINUE = new("continue");
        internal static FixedToken OF = new("of");
        internal static FixedToken TRY = new("try");
        internal static FixedToken CATCH = new("catch");
        internal static FixedToken FINALLY = new("finally");
        internal static FixedToken THROW = new("throw");
        internal static FixedToken TRUE = new("true");
        internal static FixedToken FALSE = new("false");


        internal static FixedToken END = new("[End]");
        internal static FixedToken LEFT_PARENTHESIS = new("(");
        internal static FixedToken RIGHT_PARENTHESIS = new(")");
        internal static FixedToken LEFT_BRACE = new("{");
        internal static FixedToken RIGHT_BRACE = new("}");
        internal static FixedToken SEMICOLON = new(";");
        internal static FixedToken COLON = new(":");
        internal static FixedToken COMMA = new(",");
        internal static FixedToken LEFT_BRACKET = new("[");
        internal static FixedToken RIGHT_BRACKET = new("]");
        internal static FixedToken DOUBLE_COLON = new("::");
        internal static FixedToken EQUAL = new("=");
        internal static FixedToken PLUS_EQUAL = new("+=");
        internal static FixedToken MINUS_EQUAL = new("-=");
        internal static FixedToken STAR_EQUAL = new("*=");
        internal static FixedToken SLASH_EQUAL = new("/=");
        internal static FixedToken PERCENT_EQUAL = new("%=");
        internal static FixedToken AND_EQUAL = new("&=");
        internal static FixedToken VERTICAL_EQUAL = new("|=");
        internal static FixedToken CARET_EQUAL = new("^=");
        internal static FixedToken DOUBLE_LESS_EQUAL = new("<<=");
        internal static FixedToken DOUBLE_GREATER_EQUAL = new(">>=");
        internal static FixedToken DOUBLE_VERTICAL = new("||");
        internal static FixedToken DOUBLE_AND = new("&&");
        internal static FixedToken DOUBLE_EQUAL = new("==");
        internal static FixedToken EXCLAMATION_EQUAL = new("!=");
        internal static FixedToken LESS = new("<");
        internal static FixedToken GREATER = new(">");
        internal static FixedToken LESS_EQUAL = new("<=");
        internal static FixedToken GREATER_EQUAL = new(">=");
        internal static FixedToken VERTICAL = new("|");
        internal static FixedToken CARET = new("^");
        internal static FixedToken AND = new("&");
        internal static FixedToken DOUBLE_LESS = new("<<");
        internal static FixedToken DOUBLE_GREATER = new(">>");
        internal static FixedToken PLUS = new("+");
        internal static FixedToken MINUS = new("-");
        internal static FixedToken STAR = new("*");
        internal static FixedToken SLASH = new("/");
        internal static FixedToken PERCENT = new("%");
        internal static FixedToken EXCLAMATION = new("!");

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
