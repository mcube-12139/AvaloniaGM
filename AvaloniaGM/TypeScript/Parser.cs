using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Items;
using AvaloniaGM.TypeScript.Parameters;
using AvaloniaGM.TypeScript.Patterns;
using AvaloniaGM.TypeScript.Statements;
using AvaloniaGM.TypeScript.Tokens;
using AvaloniaGM.TypeScript.TypeNodes;
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
            // 跳过空白符与注释
            for (; ; ) {
                if (c == ' ' || c == '\r' || c == '\n' || c == '\t') {
                    NextChar();
                } else if (c == '/') {
                    tokenPosition = charPosition;
                    NextChar();
                    if (c == '/') {
                        // 行注释
                        NextChar();
                        for (; ; ) {
                            if (c == '\n') {
                                NextChar();
                                break;
                            }
                            if (c == '\0') {
                                break;
                            }

                            NextChar();
                        }
                    } else if (c == '*') {
                        // 块注释
                        NextChar();
                        for (; ; ) {
                            if (c == '\0') {
                                throw CharCannotBeHere();
                            }

                            if (c == '*') {
                                NextChar();
                                if (c == '/') {
                                    NextChar();
                                    break;
                                }
                            } else {
                                NextChar();
                            }
                        }
                    } else if (c == '=') {
                        token = FixedToken.SLASH_EQUAL;
                        NextChar();
                        return;
                    } else {
                        token = FixedToken.SLASH;
                        return;
                    }
                } else {
                    break;
                }
            }

            tokenPosition = charPosition;
            if (char.IsAsciiLetter(c) || c == '_') {
                // 标识符 | 标签
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
                if (name != "l" || c != '\'') {
                    token = (IToken?)FixedToken.GetKeyword(name) ?? new IdentifierToken(name);
                } else {
                    NextChar();
                    builder.Clear();

                    if (!char.IsAsciiLetter(c) && c != '_') {
                        throw CharCannotBeHere();
                    }
                    for (; ; ) {
                        builder.Append(c);
                        NextChar();

                        if (!char.IsAsciiLetterOrDigit(c) && c != '_') {
                            break;
                        }
                    }

                    token = new LabelToken(builder.ToString());
                }
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

        void AssertTokenKind<T>()
            where T: IToken {
            if (token is not T) {
                throw TokenCannotBeHere();
            }
        }

        bool IsExpressionStart() {
            return token == FixedToken.TRUE
                || token == FixedToken.FALSE
                || token == FixedToken.LEFT_BRACKET
                || token is IdentifierToken
                || token is StringToken
                || token is IntegerToken;
        }

        IExpression ParseExpression(int priority) {
            TextPosition position = tokenPosition;
            IExpression result;

            if (token == FixedToken.TRUE) {
                result = new BooleanExpression(position, true);
                NextToken();
            } else if (token == FixedToken.FALSE) {
                result = new BooleanExpression(position, false);
                NextToken();
            } else if (token == FixedToken.LEFT_BRACKET) {
                // 数组
                NextToken();

                List<IExpression> elements = [];
                ITypeNode? type;
                if (token == FixedToken.RIGHT_BRACKET) {
                    NextToken();
                    AssertAndNextToken(FixedToken.DOUBLE_COLON);
                    AssertAndNextToken(FixedToken.LESS);
                    type = ParseType();
                    AssertAndNextToken(FixedToken.GREATER);
                } else {
                    type = null;
                    for (; ; ) {
                        elements.Add(ParseExpression(0));

                        if (token == FixedToken.COMMA) {
                            NextToken();
                        } else if (token == FixedToken.RIGHT_BRACKET) {
                            NextToken();
                            break;
                        } else {
                            throw TokenCannotBeHere();
                        }
                    }
                }

                result = new ArrayExpression(position, [.. elements], type);
            } else if (token is IdentifierToken idToken) {
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
                        // 二元
                        NextToken();
                        IExpression right = ParseExpression(op.priority);
                        result = new BinaryExpression(position, result, right, op);
                    } else if (token == FixedToken.LEFT_PARENTHESIS) {
                        // 调用
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
                    } else if (token == FixedToken.LEFT_BRACKET) {
                        // 索引
                        NextToken();
                        IExpression index = ParseExpression(0);
                        AssertAndNextToken(FixedToken.RIGHT_BRACKET);
                        result = new IndexExpression(position, result, index);
                    } else {
                        break;
                    }
                } else {
                    break;
                }
            }

            return result;
        }

        bool IsPatternStart() {
            return token is IdentifierToken;
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

        IfStatement ParseIfStatement() {
            TextPosition position = tokenPosition;

            AssertAndNextToken(FixedToken.IF);

            AssertAndNextToken(FixedToken.LEFT_PARENTHESIS);
            IExpression condition = ParseExpression(0);
            AssertAndNextToken(FixedToken.RIGHT_PARENTHESIS);

            BlockStatement body = ParseBlockStatement();

            IStatement? elseBody;
            if (token == FixedToken.ELSE) {
                NextToken();
                if (token == FixedToken.IF) {
                    elseBody = ParseIfStatement();
                } else {
                    elseBody = ParseBlockStatement();
                }
            } else {
                elseBody = null;
            }

            return new IfStatement(position, condition, body, elseBody);
        }

        BlockStatement ParseBlockStatement() {
            TextPosition position = tokenPosition;

            AssertAndNextToken(FixedToken.LEFT_BRACE);

            List<IStatement> statements = [];
            for (; ; ) {
                if (token == FixedToken.RIGHT_BRACE) {
                    NextToken();
                    break;
                }

                statements.Add(ParseStatement());
            }

            return new BlockStatement(position, [.. statements]);
        }

        bool IsStatementStart() {
            return token == FixedToken.LET
                || token == FixedToken.IF
                || token is LabelToken
                || token == FixedToken.WHILE
                || token == FixedToken.CONTINUE
                || token == FixedToken.BREAK
                || token == FixedToken.RETURN
                || token == FixedToken.LEFT_BRACE
                || IsExpressionStart();
        }

        IStatement ParseStatement() {
            TextPosition position = tokenPosition;
            IStatement result;

            if (token == FixedToken.LET) {
                // let
                NextToken();
                IPattern pattern = ParsePattern();

                ITypeNode? type;
                IExpression? initializer;
                if (token == FixedToken.EQUAL) {
                    type = null;

                    NextToken();
                    initializer = ParseExpression(0);
                } else if (token == FixedToken.COLON) {
                    NextToken();
                    type = ParseType();

                    if (token == FixedToken.EQUAL) {
                        NextToken();
                        initializer = ParseExpression(0);
                    } else {
                        initializer = null;
                    }
                } else {
                    throw TokenCannotBeHere();
                }

                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new LetStatement(position, pattern, type, initializer);
            } else if (token == FixedToken.IF) {
                // if
                result = ParseIfStatement();
            } else if (token is LabelToken || token == FixedToken.WHILE) {
                // while
                string? label;
                if (token is LabelToken labelToken) {
                    label = labelToken.name;
                    NextToken();
                    AssertAndNextToken(FixedToken.COLON);
                } else {
                    label = null;
                }

                AssertAndNextToken(FixedToken.WHILE);

                AssertAndNextToken(FixedToken.LEFT_PARENTHESIS);
                IExpression condition = ParseExpression(0);
                AssertAndNextToken(FixedToken.RIGHT_PARENTHESIS);

                BlockStatement body = ParseBlockStatement();

                result = new WhileStatement(position, label, condition, body);
            } else if (token == FixedToken.CONTINUE) {
                // continue
                NextToken();

                string? label;
                if (token is LabelToken labelToken) {
                    label = labelToken.name;
                    NextToken();
                } else {
                    label = null;
                }

                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new ContinueStatement(position, label);
            } else if (token == FixedToken.BREAK) {
                // break
                NextToken();

                string? label;
                if (token is LabelToken labelToken) {
                    label = labelToken.name;
                    NextToken();
                } else {
                    label = null;
                }

                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new BreakStatement(position, label);
            } else if (token == FixedToken.RETURN) {
                // return
                NextToken();

                IExpression? expression;
                if (IsExpressionStart()) {
                    expression = ParseExpression(0);
                } else {
                    expression = null;
                }

                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new ReturnStatement(position, expression);
            } else if (token == FixedToken.LEFT_BRACE) {
                // 块
                result = ParseBlockStatement();
            } else if (IsExpressionStart()) {
                // 算式
                IExpression expression = ParseExpression(0);
                AssertAndNextToken(FixedToken.SEMICOLON);
                result = new ExpressionStatement(expression);
            } else {
                throw TokenCannotBeHere();
            }

            return result;
        }

        IParameter ParseParameter() {
            TextPosition position = tokenPosition;
            IParameter result;

            if (token == FixedToken.THIS) {
                result = new ThisParameter(position);
            } else if (IsPatternStart()) {
                IPattern pattern = ParsePattern();

                AssertAndNextToken(FixedToken.COLON);
                ITypeNode type = ParseType();

                result = new PatternParameter(position, pattern, type);
            } else {
                throw TokenCannotBeHere();
            }

            return result;
        }

        ITypeNode ParseType() {
            TextPosition position = tokenPosition;
            ITypeNode result;

            bool fromGlobal;
            if (token == FixedToken.DOUBLE_COLON) {
                fromGlobal = true;
                NextToken();
            } else {
                fromGlobal = false;
            }

            List<string> segments = [];
            for (; ; ) {
                if (token is IdentifierToken idToken) {
                    segments.Add(idToken.name);
                    NextToken();
                } else {
                    throw TokenCannotBeHere();
                }

                if (token == FixedToken.DOUBLE_COLON) {
                    NextToken();
                } else {
                    break;
                }
            }

            result = new PathType(position, fromGlobal, [.. segments]);
            return result;
        }

        bool IsItemStart() {
            return token == FixedToken.FUNCTION;
        }

        IItem ParseItem() {
            TextPosition position = tokenPosition;
            IItem result;

            if (token == FixedToken.FUNCTION) {
                NextToken();

                if (token is not IdentifierToken idToken) {
                    throw TokenCannotBeHere();
                }
                string name = idToken.name;
                NextToken();

                AssertAndNextToken(FixedToken.LEFT_PARENTHESIS);
                List<IParameter> parameters = [];
                for (; ; ) {
                    if (token == FixedToken.RIGHT_PARENTHESIS) {
                        NextToken();
                        break;
                    }

                    parameters.Add(ParseParameter());

                    if (token == FixedToken.COMMA) {
                        NextToken();
                    } else if (token == FixedToken.RIGHT_PARENTHESIS) {
                        NextToken();
                        break;
                    } else {
                        throw TokenCannotBeHere();
                    }
                }

                ITypeNode? type;
                if (token == FixedToken.COLON) {
                    NextToken();
                    type = ParseType();
                } else {
                    type = null;
                }

                BlockStatement body = ParseBlockStatement();
                result = new FunctionItem(position, name, [.. parameters], type, body);
            } else {
                throw TokenCannotBeHere();
            }

            return result;
        }

        CodeRoot ParseRoot() {
            List<IStatement> statements = [];
            List<IItem> items = [];

            for (; ; ) {
                if (IsStatementStart()) {
                    statements.Add(ParseStatement());
                } else if (IsItemStart()) {
                    items.Add(ParseItem());
                } else {
                    break;
                }
            }
            AssertToken(FixedToken.END);

            return new([..items], [.. statements]);
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
