using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Patterns;
using AvaloniaGM.TypeScript.Statements;
using AvaloniaGM.TypeScript.Tokens;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaGM.TypeScript {
    internal class Parser {
        string source = string.Empty;
        string code = string.Empty;
        int index;
        char c;
        TextPosition indexPosition;
        TextPosition charPosition;
        TextPosition tokenPosition;
        IToken token = FixedToken.END;
        readonly StringBuilder builder = new();

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
            if (c != '\0') {
                return new SyntaxException(SyntaxErrorType.CANNOT_BE_HERE, [c.ToString()], source, charPosition);
            }

            return new SyntaxException(SyntaxErrorType.CANNOT_END_HERE, [], source, charPosition);
        }

        SyntaxException TokenCannotBeHere() {
            if (token != FixedToken.END) {
                return new SyntaxException(SyntaxErrorType.CANNOT_BE_HERE, [token.GetAppearance()], source, tokenPosition);
            }

            return new SyntaxException(SyntaxErrorType.CANNOT_END_HERE, [], source, tokenPosition);
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

                string name = builder.ToString();
                token = (IToken?)FixedToken.GetKeyword(name) ?? new IdentifierToken(name);
            } else if (char.IsAsciiDigit(c)) {
                // 数字
                builder.Clear();
                for (; ; ) {
                    builder.Append(c);
                    NextChar();

                    if (!char.IsAsciiDigit(c)) {
                        break;
                    }
                }

                token = new IntegerToken(builder.ToString());
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
            } else if (c == '[') {
                token = FixedToken.LEFT_BRACKET;
                NextChar();
            } else if (c == ']') {
                token = FixedToken.RIGHT_BRACKET;
                NextChar();
            } else if (c == '{') {
                token = FixedToken.LEFT_BRACE;
                NextChar();
            } else if (c == '}') {
                token = FixedToken.RIGHT_BRACE;
                NextChar();
            } else if (c == ';') {
                token = FixedToken.SEMICOLON;
                NextChar();
            } else if (c == ',') {
                token = FixedToken.COMMA;
                NextChar();
            } else if (c == ':') {
                NextChar();
                if (c == ':') {
                    token = FixedToken.DOUBLE_COLON;
                    NextChar();
                } else {
                    token = FixedToken.COLON;
                }
            } else if (c == '=') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.DOUBLE_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.EQUAL;
                }
            } else if (c == '+') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.PLUS_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.PLUS;
                }
            } else if (c == '-') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.MINUS_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.MINUS;
                }
            } else if (c == '*') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.STAR_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.STAR;
                }
            } else if (c == '/') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.SLASH_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.SLASH;
                }
            } else if (c == '%') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.PERCENT_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.PERCENT;
                }
            } else if (c == '&') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.AND_EQUAL;
                    NextChar();
                } else if (c == '&') {
                    token = FixedToken.DOUBLE_AND;
                    NextChar();
                } else {
                    token = FixedToken.AND;
                }
            } else if (c == '|') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.VERTICAL_EQUAL;
                    NextChar();
                } else if (c == '|') {
                    token = FixedToken.DOUBLE_VERTICAL;
                    NextChar();
                } else {
                    token = FixedToken.VERTICAL;
                }
            } else if (c == '^') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.CARET_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.CARET;
                }
            } else if (c == '<') {
                NextChar();
                if (c == '<') {
                    NextChar();
                    if (c == '=') {
                        token = FixedToken.DOUBLE_LESS_EQUAL;
                        NextChar();
                    } else {
                        token = FixedToken.DOUBLE_LESS;
                    }
                } else if (c == '=') {
                    token = FixedToken.LESS_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.LESS;
                }
            } else if (c == '>') {
                NextChar();
                if (c == '>') {
                    NextChar();
                    if (c == '=') {
                        token = FixedToken.DOUBLE_GREATER_EQUAL;
                        NextChar();
                    } else {
                        token = FixedToken.DOUBLE_GREATER;
                    }
                } else if (c == '=') {
                    token = FixedToken.GREATER_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.GREATER;
                }
            } else if (c == '!') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.EXCLAMATION_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.EXCLAMATION;
                }
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
                || token is StringToken
                || token is IntegerToken;
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
            } else if (token is IntegerToken intToken) {
                result = new IntegerExpression(position, intToken.valueStr);
                NextToken();
            } else {
                throw TokenCannotBeHere();
            }
            
            for (; ; ) {
                if (token is FixedToken fixedToken) {
                    var op = BinaryOperator.FromToken(priority, fixedToken);
                    if (op != null) {
                        NextToken();
                        IExpression right = ParseExpression(op.priority);
                        result = new BinaryExpression(position, result, right, op);
                    } else if (token == FixedToken.LEFT_PARENTHESIS) {
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
                } else {
                    break;
                }
            }

            return result;
        }

        IPattern ParsePattern() {
            TextPosition position = tokenPosition;
            IPattern result;

            if (token is IdentifierToken idToken) {
                result = new IdentifierPattern(position, idToken.name);
                NextToken();
            } else {
                throw TokenCannotBeHere();
            }

            return result;
        }

        bool IsStatementStart() {
            return token == FixedToken.LET
                || token == FixedToken.LEFT_BRACE
                || IsExpressionStart();
        }

        IStatement ParseStatement() {
            TextPosition position = tokenPosition;
            IStatement result;

            if (token == FixedToken.LET) {
                NextToken();
                IPattern pattern = ParsePattern();

                IExpression? initializer;
                if (token == FixedToken.EQUAL) {
                    NextToken();
                    initializer = ParseExpression(0);
                } else if (token == FixedToken.COLON) {
                    initializer = null;
                } else {
                    throw TokenCannotBeHere();
                }

                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new LetStatement(position, pattern, null, initializer);
            } else if (token == FixedToken.LEFT_BRACE) {
                NextToken();

                List<IStatement> statements = [];
                for (; ; ) {
                    if (token == FixedToken.RIGHT_BRACE) {
                        NextToken();
                        break;
                    }

                    statements.Add(ParseStatement());
                }

                result = new BlockStatement(position, [.. statements]);
            } else if (IsExpressionStart()) {
                IExpression expression = ParseExpression(0);
                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new ExpressionStatement(expression);
            } else {
                throw TokenCannotBeHere();
            }

            return result;
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

        internal CodeRoot Parse(string source, string code) {
            this.source = source;
            this.code = code;

            index = 0;
            indexPosition = new(1, 1);
            NextChar();
            NextToken();
            return ParseRoot();
        }
    }
}
