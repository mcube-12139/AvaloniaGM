using System.Text;
using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Parameter;
using TypeScript.Pattern;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.Token;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript;

public class Compiler {
    readonly List<(string name, string content)> files = [];
    string source = string.Empty;
    string code = string.Empty;
    int index;
    char c;
    TextPosition indexPosition;
    TextPosition charPosition;
    TextPosition tokenPosition;
    IToken token = FixedToken.END;
    readonly StringBuilder builder = new();

    readonly List<NameSpace> spaces;
    NameSpace space;
    readonly List<ILoopStatement> loops = [];
    readonly Stack<FunctionContext> functionContexts = [];
    IType resultType = TupleType.EMPTY;

    public Compiler() {
        NameSpace space = new();
        spaces = [space];
        this.space = space;
    }

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
        where T : IToken {
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
        } else /*if (token == FixedToken.LEFT_BRACKET) {
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
        } else */if (token is IdentifierToken idToken) {
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
            if (token is FixedToken) {
                if (priority < 12 && token == FixedToken.STAR) {
                    NextToken();
                    IExpression right = ParseExpression(12);
                    result = new MultiplyExpression(position, result, right);
                } else if (priority < 12 && token == FixedToken.SLASH) {
                    NextToken();
                    IExpression right = ParseExpression(12);
                    result = new DivideExpression(position, result, right);
                } else if (priority < 12 && token == FixedToken.PERCENT) {
                    NextToken();
                    IExpression right = ParseExpression(12);
                    result = new ModuloExpression(position, result, right);
                } else if (priority < 11 && token == FixedToken.PLUS) {
                    NextToken();
                    IExpression right = ParseExpression(12);
                    result = new AddExpression(position, result, right);
                } else if (priority < 11 && token == FixedToken.MINUS) {
                    NextToken();
                    IExpression right = ParseExpression(11);
                    result = new SubtractExpression(position, result, right);
                } else if (priority < 10 && token == FixedToken.DOUBLE_LESS) {
                    NextToken();
                    IExpression right = ParseExpression(10);
                    result = new LeftShiftExpression(position, result, right);
                } else if (priority < 10 && token == FixedToken.DOUBLE_GREATER) {
                    NextToken();
                    IExpression right = ParseExpression(10);
                    result = new RightShiftExpression(position, result, right);
                } else if (priority < 9 && token == FixedToken.AND) {
                    NextToken();
                    IExpression right = ParseExpression(9);
                    result = new BitAndExpression(position, result, right);
                } else if (priority < 8 && token == FixedToken.CARET) {
                    NextToken();
                    IExpression right = ParseExpression(8);
                    result = new BitXorExpression(position, result, right);
                } else if (priority < 7 && token == FixedToken.VERTICAL) {
                    NextToken();
                    IExpression right = ParseExpression(7);
                    result = new BitOrExpression(position, result, right);
                } else if (priority < 6 && token == FixedToken.GREATER) {
                    NextToken();
                    IExpression right = ParseExpression(6);
                    result = new GreaterExpression(position, result, right);
                } else if (priority < 6 && token == FixedToken.GREATER_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(6);
                    result = new GreaterEqualExpression(position, result, right);
                } else if (priority < 6 && token == FixedToken.LESS) {
                    NextToken();
                    IExpression right = ParseExpression(6);
                    result = new LessExpression(position, result, right);
                } else if (priority < 6 && token == FixedToken.LESS_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(6);
                    result = new LessEqualExpression(position, result, right);
                } else if (priority < 6 && token == FixedToken.DOUBLE_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(6);
                    result = new EqualExpression(position, result, right);
                } else if (priority < 6 && token == FixedToken.EXCLAMATION_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(6);
                    result = new NotEqualExpression(position, result, right);
                } else if (priority < 5 && token == FixedToken.DOUBLE_AND) {
                    NextToken();
                    IExpression right = ParseExpression(5);
                    result = new AndExpression(position, result, right);
                } else if (priority < 4 && token == FixedToken.DOUBLE_VERTICAL) {
                    NextToken();
                    IExpression right = ParseExpression(4);
                    result = new OrExpression(position, result, right);
                } else if (priority <= 2 && token == FixedToken.EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new AssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.PLUS_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new AddAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.MINUS_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new SubtractAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.STAR_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new MultiplyAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.SLASH_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new DivideAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.PERCENT_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new ModuloAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.AND_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new BitAndAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.VERTICAL_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new BitOrAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.CARET_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new BitXorAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.DOUBLE_LESS_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new LeftShiftAssignExpression(position, result.AsPlace(this), right);
                } else if (priority <= 2 && token == FixedToken.DOUBLE_GREATER_EQUAL) {
                    NextToken();
                    IExpression right = ParseExpression(2);
                    result = new RightShiftAssignExpression(position, result.AsPlace(this), right);
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
                } else /*if (token == FixedToken.LEFT_BRACKET) {
                    // 索引
                    NextToken();
                    IExpression index = ParseExpression(0);
                    AssertAndNextToken(FixedToken.RIGHT_BRACKET);
                    result = new IndexExpression(position, result, index);
                } else */{
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
        } else /*if (token == FixedToken.CONTINUE) {
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
        } else */if (token == FixedToken.LEFT_BRACE) {
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
        /*
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
        */
        throw new NotImplementedException();
    }

    ITypeNode ParseType() {
        /*
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
        */
        throw new NotImplementedException();
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

    FileNode ParseRoot() {
        List<IStatement> statements = [];
        List<IItem> items = [];

        for (; ; ) {
            if (IsItemStart()) {
                items.Add(ParseItem());
            } else {
                break;
            }
        }
        AssertToken(FixedToken.END);

        return new([.. items]);
    }

    internal SemanticException SemanticError(SemanticErrorType type, string[] parameters, TextPosition position) {
        return new SemanticException(type, parameters, source, position);
    }

    internal void AddSymbol(string name, ISymbol symbol, TextPosition position) {
        if (!space.TryAddSymbol(name, symbol)) {
            throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], source, position);
        }
    }

    internal ISymbol GetSymbol(string name, TextPosition position) {
        for (int i = spaces.Count - 1; i != -1; --i) {
            ISymbol? symbol = spaces[i].GetSymbol(name);
            if (symbol != null) {
                return symbol;
            }
        }

        throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [name], source, position);
    }

    internal void EnterNameSpace() {
        space = new();
        spaces.Add(space);
    }

    internal void LeaveNameSpace() {
        spaces.RemoveAt(spaces.Count - 1);
        space = spaces[^1];
    }

    internal void EnterLoop(string? label, ILoopStatement statement) {
        loops.Add(statement);
        if (label != null) {
            space.TryAddLoop(label, statement);
        }
    }

    internal void LeaveLoop() {
        loops.RemoveAt(loops.Count - 1);
    }

    internal void EnterFunction(IType resultType) {
        functionContexts.Push(new(resultType));

        this.resultType = resultType;

        EnterNameSpace();
    }

    internal void LeaveFunction() {
        FunctionContext context = functionContexts.Pop();

        resultType = context.resultType;

        LeaveNameSpace();
    }

    public FileNode[] Compile(ICollection<(string name, string content)> files) {
        this.files.AddRange(files);

        space.TryAddSymbol("printInt", FunctionSymbol.NewResolved("printInt", new FunctionType([PrimitiveType.INTEGER], PrimitiveType.INTEGER), "show_message"));
        space.TryAddSymbol("printString", FunctionSymbol.NewResolved("printString", new FunctionType([PrimitiveType.STRING], PrimitiveType.INTEGER), "show_message"));

        List<FileNode> nodes = [];
        foreach ((string name, string content) in files) {
            source = name;
            code = content;

            index = 0;
            indexPosition = new(1, 1);
            NextChar();
            NextToken();
            nodes.Add(ParseRoot());
        }

        foreach (FileNode node in nodes) {
            node.SetSemantic(this);
        }

        this.files.Clear();
        return [.. nodes];
    }
}
