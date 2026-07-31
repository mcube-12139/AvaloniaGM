using AvaloniaGM.Entities;
using AvaloniaGM.Entities.Expressions;
using AvaloniaGM.Entities.Statement;
using AvaloniaGM.Entities.Token;
using AvaloniaGM.Exceptions;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaGM.Services {
    internal class CodeParser {
        string code = string.Empty;
        int index;
        char c;
        TextPosition indexPosition;
        TextPosition charPosition;
        TextPosition tokenPosition;
        IToken token = FixedToken.END;
        StringBuilder builder = new();

        void NextChar() {
            if (index != code.Length) {
                c = code[index];

                charPosition = indexPosition;
                if (c != '\n') {
                    ++indexPosition.column;
                } else {
                    ++indexPosition.line;
                    indexPosition.column = 1;
                }
                ++index;
            } else {
                c = '\0';
            }
        }

        SyntaxException CharCannotBeHere() {
            return new SyntaxException();
        }

        SyntaxException TokenCannotBeHere() {
            return new SyntaxException();
        }

        void NextToken() {
            // 跳过空白符
            for (; ; ) {
                if (c != ' ' && c != '\r' && c != '\n' && c != '\t') {
                    break;
                }

                NextChar();
            }

            tokenPosition = charPosition;
            if (char.IsAsciiLetter(c) || c == '_') {
                // 标识符
                builder.Clear().Append(c);
                NextChar();

                for (; ; ) {
                    if (!char.IsAsciiLetterOrDigit(c) && c != '_') {
                        break;
                    }

                    builder.Append(c);
                    NextChar();
                }

                token = new IdentifierToken(builder.ToString());
            } else if (c == '"') {
                // 字符串
                NextChar();
                builder.Clear();

                for (; ; ) {
                    if (c == '"') {
                        NextChar();
                        break;
                    }
                    if (c == '\0') {
                        throw CharCannotBeHere();
                    }

                    builder.Append(c);
                    NextChar();
                }

                token = new StringToken(builder.ToString());
            } else if (c == '(') {
                token = FixedToken.LEFT_PARENTHESIS;
                NextChar();
            } else if (c == ')') {
                token = FixedToken.RIGHT_PARENTHESIS;
                NextChar();
            } else if (c == ';') {
                token = FixedToken.SEMICOLON;
                NextChar();
            } else if (c == ',') {
                token = FixedToken.COMMA;
                NextChar();
            } else if (c == '\0') {
                token = FixedToken.END;
                NextChar();
            } else {
                throw CharCannotBeHere();
            }
        }

        void AssertAndNextToken(FixedToken expected) {
            if (token != expected) {
                throw TokenCannotBeHere();
            }

            NextToken();
        }

        void AssertToken(FixedToken expected) {
            if (token != expected) {
                throw TokenCannotBeHere();
            }
        }

        bool IsExpressionStart() {
            return token is IdentifierToken
                || token is StringToken;
        }

        IExpression ParseExpression(int priority) {
            TextPosition position = tokenPosition;
            IExpression result;

            if (token is IdentifierToken idToken) {
                result = new PathExpression(position, [idToken.name]);
                NextToken();
            } else if (token is StringToken strToken) {
                result = new StringExpression(position, strToken.value);
                NextToken();
            } else {
                throw TokenCannotBeHere();
            }
            
            for (; ; ) {
                if (token == FixedToken.LEFT_PARENTHESIS) {
                    NextToken();
                    List<IExpression> parameters = [];
                    for (; ; ) {
                        if (token == FixedToken.RIGHT_PARENTHESIS) {
                            NextToken();
                            break;
                        }

                        parameters.Add(ParseExpression(0));

                        if (token == FixedToken.COMMA) {
                            NextToken();
                        } else if (token == FixedToken.RIGHT_PARENTHESIS) {
                            NextToken();
                            break;
                        } else {
                            throw TokenCannotBeHere();
                        }
                    }

                    result = new CallExpression(position, result, [.. parameters]);
                } else {
                    break;
                }
            }

            return result;
        }

        bool IsStatementStart() {
            return IsExpressionStart();
        }

        IStatement ParseStatement() {
            IExpression expression = ParseExpression(0);
            AssertAndNextToken(FixedToken.SEMICOLON);
            return new ExpressionStatement(expression);
        }

        CodeRoot ParseRoot() {
            List<IStatement> statements = [];

            for (; ; ) {
                if (IsStatementStart()) {
                    statements.Add(ParseStatement());
                } else {
                    break;
                }
            }
            AssertToken(FixedToken.END);

            return new([.. statements]);
        }

        internal CodeRoot Parse(string code) {
            this.code = code;

            index = 0;
            indexPosition = new(1, 1);
            NextChar();
            NextToken();
            return ParseRoot();
        }
    }
}
