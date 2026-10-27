using System.Text;
using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.FlowContext;
using TypeScript.GenericArgumentNode;
using TypeScript.GenericParameter;
using TypeScript.Item;
using TypeScript.Parameter;
using TypeScript.PathSegment;
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
    SourcePosition tokenPosition;
    IToken token = FixedToken.END;
    readonly StringBuilder builder = new();

    readonly Stack<NameSpace> spaces = [];
    readonly NameSpace topSpace;
    internal NameSpace space;
    readonly List<ILoopStatement> loops = [];
    readonly Stack<IFlowContext> flowContexts = [];
    readonly Stack<IType> thisTypes = [];
    IType? thisType;
    readonly Stack<FunctionContext> functionContexts = [];
    internal ThisParameterSymbol? thisParameterSymbol;
    IType resultType = TupleType.EMPTY;
    bool returned;
    internal bool inInterface;

    public Compiler() {
        NameSpace space = new(null);
        topSpace = space;
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
            return new SyntaxException(SyntaxErrorType.CANNOT_BE_HERE, [c.ToString()], source, new(source, charPosition.line, charPosition.column));
        }

        return new SyntaxException(SyntaxErrorType.CANNOT_END_HERE, [], source, new(source, charPosition.line, charPosition.column));
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
                tokenPosition.line = charPosition.line;
                tokenPosition.column = charPosition.column;
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

        tokenPosition.line = charPosition.line;
        tokenPosition.column = charPosition.column;
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
        } else if (c == '.') {
            NextChar();
            if (c == '.') {
                NextChar();
                if (c == '=') {
                    token = FixedToken.DOUBLE_FULL_STOP_EQUAL;
                    NextChar();
                } else {
                    token = FixedToken.DOUBLE_FULL_STOP;
                }
            } else {
                token = FixedToken.FULL_STOP;
            }
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

    string GetIdentifierNameAndNextToken() {
        if (token is not IdentifierToken idToken) {
            throw TokenCannotBeHere();
        }
        string name = idToken.name;
        NextToken();

        return name;
    }

    NewExpression.Field ParseNewExpressionField() {
        SourcePosition position = tokenPosition;

        string name = GetIdentifierNameAndNextToken();

        AssertAndNextToken(FixedToken.COLON);

        IExpression value = ParseExpression(0);

        return new(position, name, value);
    }

    IExpression[] ParseCommaSeperatedExpressions() {
        List<IExpression> parameters = [];
        if (IsExpressionStart()) {
            for (; ; ) {
                parameters.Add(ParseExpression(0));

                if (token == FixedToken.COMMA) {
                    NextToken();
                    if (!IsExpressionStart()) {
                        break;
                    }
                } else if (!IsExpressionStart()) {
                    break;
                }
            }
        }

        return [.. parameters];
    }

    bool IsPathExpressionStart() {
        return token is IdentifierToken || token == FixedToken.THIS || token == FixedToken.DOUBLE_COLON;
    }

    IPathSegment ParsePathSegment() {
        SourcePosition position = tokenPosition;
        IPathSegment result;

        if (token is IdentifierToken idToken) {
            result = new IdentifierSegment(position, idToken.name);
            NextToken();
        } else if (token == FixedToken.THIS) {
            result = new ThisSegment(position);
            NextToken();
        } else {
            throw TokenCannotBeHere();
        }

        return result;
    }

    bool IsExpressionStart() {
        return token == FixedToken.TRUE
            || token == FixedToken.FALSE
            || token == FixedToken.MINUS
            || token == FixedToken.EXCLAMATION
            || token == FixedToken.LEFT_BRACKET
            || token == FixedToken.NEW
            || IsPathExpressionStart()
            || token is StringToken
            || token is IntegerToken;
    }

    IExpression ParseExpression(int priority) {
        SourcePosition position = tokenPosition;
        IExpression result;

        if (token == FixedToken.TRUE) {
            result = new BooleanExpression(position, true);
            NextToken();
        } else if (token == FixedToken.FALSE) {
            result = new BooleanExpression(position, false);
            NextToken();
        } else if (token == FixedToken.MINUS) {
            NextToken();
            IExpression expression = ParseExpression(14);
            result = new NegateExpression(position, expression);
        } else if (token == FixedToken.EXCLAMATION) {
            NextToken();
            IExpression expression = ParseExpression(14);
            result = new NotExpression(position, expression);
        } else if (token == FixedToken.LEFT_BRACKET) {
            // 数组
            NextToken();
            IExpression[] elements = ParseCommaSeperatedExpressions();
            AssertAndNextToken(FixedToken.RIGHT_BRACKET);
            result = new ArrayExpression(position, [.. elements]);
        } else if (IsPathExpressionStart()) {
            // 路径
            bool fromTop;
            if (token == FixedToken.DOUBLE_COLON) {
                fromTop = true;
                NextToken();
            } else {
                fromTop = false;
            }

            List<GenericPathSegment> segments = [];
            for (; ; ) {
                segments.Add(ParseExpressionPathSegment());

                if (token == FixedToken.DOUBLE_COLON) {
                    NextToken();
                } else {
                    break;
                }
            }

            result = new Path(position, [.. segments], fromTop);
        } else if (token == FixedToken.NEW) {
            NextToken();

            Path pathType = ParsePathType();

            AssertAndNextToken(FixedToken.LEFT_BRACE);
            List<NewExpression.Field> fields = [];
            for (; ; ) {
                if (token == FixedToken.RIGHT_BRACE) {
                    NextToken();
                    break;
                }

                fields.Add(ParseNewExpressionField());

                if (token == FixedToken.COMMA) {
                    NextToken();
                } else if (token == FixedToken.RIGHT_BRACE) {
                    NextToken();
                    break;
                } else {
                    throw TokenCannotBeHere();
                }
            }

            result = new NewExpression(position, pathType, [.. fields]);
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
                    IExpression[] parameters = ParseCommaSeperatedExpressions();
                    AssertAndNextToken(FixedToken.RIGHT_PARENTHESIS);

                    result = new CallExpression(position, result, [.. parameters]);
                } else if (token == FixedToken.LEFT_BRACKET) {
                    // 索引
                    NextToken();
                    IExpression index = ParseExpression(0);
                    AssertAndNextToken(FixedToken.RIGHT_BRACKET);
                    result = new IndexExpression(position, result, index);
                } else if (token == FixedToken.FULL_STOP) {
                    NextToken();
                    string name = GetIdentifierNameAndNextToken();

                    if (token == FixedToken.LEFT_PARENTHESIS) {
                        // 调用方法
                        NextToken();
                        IExpression[] parameters = ParseCommaSeperatedExpressions();
                        AssertAndNextToken(FixedToken.RIGHT_PARENTHESIS);

                        result = new MethodCallExpression(position, result, name, parameters);
                    } else {
                        // 访问字段
                        result = new FieldAccessExpression(position, result, name);
                    }
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
        SourcePosition position = tokenPosition;
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
        SourcePosition position = tokenPosition;

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

    bool IsBlockStatementStart() {
        return token == FixedToken.LEFT_BRACE;
    }
    
    BlockStatement ParseBlockStatement() {
        SourcePosition position = tokenPosition;

        AssertAndNextToken(FixedToken.LEFT_BRACE);

        List<IItem> items = [];
        List<ImplNode> impls = [];
        List<IStatement> statements = [];
        for (; ; ) {
            if (IsStatementStart()) {
                statements.Add(ParseStatement());
            } else if (IsItemStart()) {
                items.Add(ParseItem());
            } else if (IsImplStart()) {
                impls.Add(ParseImpl());
            } else if (token == FixedToken.RIGHT_BRACE) {
                NextToken();
                break;
            } else {
                throw TokenCannotBeHere();
            }
        }

        return new BlockStatement(position, [.. items], [.. impls], [.. statements]);
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
        SourcePosition position = tokenPosition;
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

    IGenericArgumentNode ParseGenericArgument() {
        return ParseType();
    }

    bool IsGenericArgumentsStart() {
        return token == FixedToken.LESS;
    }

    IGenericArgumentNode[] ParseGenericArguments() {
        AssertAndNextToken(FixedToken.LESS);
        List<IGenericArgumentNode> result = [];
        for (; ; ) {
            result.Add(ParseGenericArgument());

            if (token == FixedToken.COMMA) {
                NextToken();
                if (token == FixedToken.GREATER) {
                    NextToken();
                    break;
                }
            } else if (token == FixedToken.GREATER) {
                NextToken();
                break;
            } else {
                throw TokenCannotBeHere();
            }
        }

        return [.. result];
    }

    GenericPathSegment ParseExpressionPathSegment() {
        SourcePosition position = tokenPosition;

        IPathSegment segment = ParsePathSegment();
        IGenericArgumentNode[]? genericArguments;
        if (token == FixedToken.DOUBLE_COLON) {
            NextToken();
            genericArguments = ParseGenericArguments();
        } else {
            genericArguments = null;
        }

        return new(position, segment, genericArguments);
    }

    GenericPathSegment ParseTypePathSegment() {
        SourcePosition position = tokenPosition;

        IPathSegment segment = ParsePathSegment();
        IGenericArgumentNode[]? genericArguments;
        if (token == FixedToken.DOUBLE_COLON) {
            NextToken();
            genericArguments = ParseGenericArguments();
        } else if (IsGenericArgumentsStart()) {
            // 类型路径的泛型实参可省略::
            genericArguments = ParseGenericArguments();
        } else {
            genericArguments = null;
        }

        return new(position, segment, genericArguments);
    }

    bool IsPathTypeStart() {
        return token == FixedToken.DOUBLE_COLON
            || token is IdentifierToken;
    }

    Path ParsePathType() {
        SourcePosition position = tokenPosition;

        bool fromTop;
        if (token == FixedToken.DOUBLE_COLON) {
            fromTop = true;
            NextToken();
        } else {
            fromTop = false;
        }

        List<GenericPathSegment> segments = [];
        for (; ; ) {
            segments.Add(ParseTypePathSegment());

            if (token == FixedToken.DOUBLE_COLON) {
                NextToken();
            } else {
                break;
            }
        }

        return new Path(position, [.. segments], fromTop);
    }

    ITypeNode ParseType() {
        SourcePosition position = tokenPosition;
        ITypeNode result;

        if (IsPathTypeStart()) {
            result = ParsePathType();
        } else {
            throw TokenCannotBeHere();
        }

        return result;
    }

    ClassItem.Field ParseClassField() {
        SourcePosition position = tokenPosition;

        if (token is not IdentifierToken idToken) {
            throw TokenCannotBeHere();
        }
        string name = idToken.name;
        NextToken();

        AssertAndNextToken(FixedToken.COLON);

        ITypeNode type = ParseType();

        AssertAndNextToken(FixedToken.SEMICOLON);

        return new(position, name, type);
    }

    bool IsThisParameterStart() {
        return token == FixedToken.THIS;
    }

    ThisParameter ParseThisParameter() {
        SourcePosition position = tokenPosition;

        AssertAndNextToken(FixedToken.THIS);

        return new(position);
    }

    bool IsFunctionParameterStart() {
        return IsPatternStart();
    }

    IParameter[] ParseFunctionParameters() {
        List<IParameter> result = [];
        if (IsPatternStart()) {
            for (; ; ) {
                SourcePosition parameterPosition = tokenPosition;
                IPattern pattern = ParsePattern();

                AssertAndNextToken(FixedToken.COLON);
                ITypeNode parameterType = ParseType();

                result.Add(new PatternParameter(parameterPosition, pattern, parameterType));

                if (token == FixedToken.COMMA) {
                    NextToken();
                    if (!IsFunctionParameterStart()) {
                        break;
                    }
                } else {
                    break;
                }
            }
        }

        return [.. result];
    }

    Path[]? TryParseBounds() {
        if (token != FixedToken.COLON) {
            return null;
        }

        NextToken();

        List<Path> result = [];
        for (; ; ) {
            result.Add(ParsePathType());

            if (token == FixedToken.AND) {
                NextToken();
            } else {
                break;
            }
        }

        return [.. result];
    }

    IGenericParameter ParseGenericParameter() {
        SourcePosition position = tokenPosition;
        IGenericParameter result;

        if (token is IdentifierToken idToken) {
            string name = idToken.name;
            NextToken();

            Path[]? bounds = TryParseBounds();

            ITypeNode? defaultType;
            if (token == FixedToken.EQUAL) {
                NextToken();
                defaultType = ParseType();
            } else {
                defaultType = null;
            }

            result = new TypeParameter(position, name, bounds, defaultType);
        } else {
            throw TokenCannotBeHere();
        }

        return result;
    }

    IGenericParameter[]? TryParseGenericParameters() {
        if (token != FixedToken.LESS) {
            return null;
        }

        NextToken();
        List<IGenericParameter> result = [];
        for (; ; ) {
            result.Add(ParseGenericParameter());

            if (token == FixedToken.COMMA) {
                NextToken();
                if (token == FixedToken.GREATER) {
                    NextToken();
                    break;
                }
            } else if (token == FixedToken.GREATER) {
                NextToken();
                break;
            } else {
                throw TokenCannotBeHere();
            }
        }

        return [.. result];
    }

    bool IsFunctionStart() {
        return token == FixedToken.FUNCTION;
    }

    FunctionItem ParseFunction() {
        SourcePosition position = tokenPosition;

        AssertAndNextToken(FixedToken.FUNCTION);

        string name = GetIdentifierNameAndNextToken();

        IGenericParameter[]? genericParameters = TryParseGenericParameters();

        AssertAndNextToken(FixedToken.LEFT_PARENTHESIS);
        ThisParameter? thisParameter;
        IParameter[] parameters;
        if (IsThisParameterStart()) {
            thisParameter = ParseThisParameter();

            if (token == FixedToken.COMMA) {
                NextToken();
                if (IsFunctionParameterStart()) {
                    parameters = ParseFunctionParameters();
                } else {
                    parameters = [];
                }
            } else {
                parameters = [];
            }
        } else if (IsFunctionParameterStart()) {
            thisParameter = null;
            parameters = ParseFunctionParameters();
        } else {
            thisParameter = null;
            parameters = [];
        }
        AssertAndNextToken(FixedToken.RIGHT_PARENTHESIS);

        ITypeNode? type;
        if (token == FixedToken.COLON) {
            NextToken();
            type = ParseType();
        } else {
            type = null;
        }

        BlockStatement? body;
        string? hostName;
        if (IsBlockStatementStart()) {
            body = ParseBlockStatement();
            hostName = null;
        } else if (token == FixedToken.HOST) {
            body = null;
            NextToken();
            if (token is not StringToken strToken) {
                throw TokenCannotBeHere();
            }
            hostName = strToken.value;
            NextToken();
            AssertAndNextToken(FixedToken.SEMICOLON);
        } else if (token == FixedToken.SEMICOLON) {
            body = null;
            hostName = null;
            NextToken();
        } else {
            throw TokenCannotBeHere();
        }

        return new(position, name, genericParameters, thisParameter, parameters, type, body, hostName);
    }

    bool IsImplStart() {
        return token == FixedToken.IMPLEMENT;
    }

    ImplNode ParseImpl() {
        SourcePosition position = tokenPosition;

        AssertAndNextToken(FixedToken.IMPLEMENT);

        ITypeNode type = ParseType();

        Path? regardedType;
        if (token == FixedToken.AS) {
            NextToken();
            regardedType = ParsePathType();
        } else {
            regardedType = null;
        }

        AssertAndNextToken(FixedToken.LEFT_BRACE);
        List<FunctionItem> functions = [];
        for (; ; ) {
            if (token == FixedToken.RIGHT_BRACE) {
                NextToken();
                break;
            }

            functions.Add(ParseFunction());
        }

        return new ImplNode(position, type, regardedType, [.. functions]);
    }

    bool IsItemStart() {
        return IsFunctionStart()
            || token == FixedToken.CLASS
            || token == FixedToken.INTERFACE
            || token == FixedToken.NAMESPACE;
    }

    IItem ParseItem() {
        SourcePosition position = tokenPosition;
        IItem result;

        if (IsFunctionStart()) {
            result = ParseFunction();
        } else if (token == FixedToken.CLASS) {
            NextToken();

            if (token is not IdentifierToken idToken) {
                throw TokenCannotBeHere();
            }
            string name = idToken.name;
            NextToken();

            AssertAndNextToken(FixedToken.LEFT_BRACE);
            List<ClassItem.Field> fields = [];
            for (; ; ) {
                if (token == FixedToken.RIGHT_BRACE) {
                    NextToken();
                    break;
                }

                fields.Add(ParseClassField());
            }

            result = new ClassItem(position, name, [.. fields]);
        } else if (token == FixedToken.INTERFACE) {
            NextToken();

            string name = GetIdentifierNameAndNextToken();

            AssertAndNextToken(FixedToken.LEFT_BRACE);
            List<FunctionItem> functions = [];
            for (; ; ) {
                if (token == FixedToken.RIGHT_BRACE) {
                    NextToken();
                    break;
                }

                functions.Add(ParseFunction());
            }

            return new InterfaceItem(position, name, [.. functions]);
        } else if (token == FixedToken.NAMESPACE) {
            NextToken();

            string name = GetIdentifierNameAndNextToken();

            AssertAndNextToken(FixedToken.LEFT_BRACE);
            (IItem[] items, ImplNode[] impls) = ParseItemAndImpls();
            AssertAndNextToken(FixedToken.RIGHT_BRACE);

            return new NameSpaceItem(position, name, items, impls);
        } else {
            throw TokenCannotBeHere();
        }

        return result;
    }

    (IItem[], ImplNode[]) ParseItemAndImpls() {
        List<IItem> items = [];
        List<ImplNode> impls = [];

        for (; ; ) {
            if (IsItemStart()) {
                items.Add(ParseItem());
            } else if (IsImplStart()) {
                impls.Add(ParseImpl());
            } else {
                break;
            }
        }

        return ([.. items], [.. impls]);
    }

    FileNode ParseRoot() {
        (IItem[] items, ImplNode[] impls) = ParseItemAndImpls();
        AssertToken(FixedToken.END);

        return new(items, impls);
    }

    internal void AddSymbol(string name, ISymbol symbol, SourcePosition position) {
        space.AddSymbol(name, symbol, position);
    }

    internal ISymbol GetSymbol(string name, SourcePosition position) {
        for (NameSpace? space = this.space; space != null; space = space.upper) {
            ISymbol? symbol = space.TryGetSymbol(name);
            if (symbol != null) {
                return symbol;
            }
        }

        throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [name], position);
    }

    internal NameSpace GetTopNameSpace() => topSpace;

    internal ILoopStatement GetLoop(string? label, SourcePosition position) {
        if (loops.Count == 0) {
            throw new SemanticException(SemanticErrorType.NOT_IN_LOOP, [], position);
        }

        if (label == null) {
            return loops[^1];
        } else {
            for (NameSpace? space = this.space; space != null; space = space.upper) {
                ILoopStatement? statement = space.TryGetLoop(label);
                if (statement != null) {
                    return statement;
                }
            }

            throw new SemanticException(SemanticErrorType.LABEL_NOT_EXIST, [label], position);
        }
    }

    internal void PushNameSpace(NameSpace space) {
        spaces.Push(this.space);
        this.space = space;
    }

    internal void PopNameSpace() {
        space = spaces.Pop();
    }

    internal NameSpace EnterNewNameSpace() {
        space = new(space);

        return space;
    }

    internal void LeaveNameSpace() {
        space = space.upper!;
    }

    internal void EnterLoop(string? label, ILoopStatement statement) {
        loops.Add(statement);
        if (label != null) {
            space.AddLoop(label, statement);
        }
    }

    internal void LeaveLoop() {
        loops.RemoveAt(loops.Count - 1);
    }

    internal void EnterFlow(IFlowContext context) {
        flowContexts.Push(context);
    }

    internal void StartElse() {
        var context = (IfFlowContext)flowContexts.Peek();
        context.StartElse();
    }

    internal void SetReturned() {
        if (flowContexts.Count == 0) {
            returned = true;
        } else {
            IFlowContext lastContext = flowContexts.Peek();
            lastContext.SetReturned();
        }
    }

    internal bool GetReturned() => returned;

    internal void AddInitializedLocal(LocalSymbol symbol) {
        if (flowContexts.Count != 0) {
            flowContexts.Peek().AddInitializedLocal(symbol);
        }
    }

    internal void LeaveFlow() {
        IFlowContext context = flowContexts.Pop();
        context.Leave();

        if (context.GetReturned()) {
            SetReturned();
        }

        foreach (LocalSymbol symbol in context.IterateInitializedLocals()) {
            AddInitializedLocal(symbol);
        }
        foreach (LocalSymbol symbol in context.IterateResetLocals()) {
            symbol.initialized = false;
        }
    }

    internal void EnterFunction(IType resultType) {
        functionContexts.Push(new(thisParameterSymbol, resultType, returned));

        thisParameterSymbol = null;
        this.resultType = resultType;
        returned = false;
    }

    internal void LeaveFunction() {
        FunctionContext context = functionContexts.Pop();

        thisParameterSymbol = context.thisParameterSymbol;
        resultType = context.resultType;
        returned = context.returned;
    }

    internal void EnterThisType(IType thisType) {
        if (thisType != null) {
            thisTypes.Push(thisType);
        }

        this.thisType = thisType;
    }

    internal void LeaveThisType() {
        if (!thisTypes.TryPop(out thisType)) {
            thisType = null;
        }
    }

    internal IType? GetThisType() => thisType;

    internal IType GetResultType() => resultType;

    public FileNode[] Compile(ICollection<(string name, string content)> files) {
        this.files.AddRange(files);

        space.AddSymbol("int", new PrimitiveTypeSymbol("int", PrimitiveType.INTEGER), tokenPosition);
        space.AddSymbol("boolean", new PrimitiveTypeSymbol("boolean", PrimitiveType.BOOLEAN), tokenPosition);
        space.AddSymbol("string", new PrimitiveTypeSymbol("string", PrimitiveType.STRING), tokenPosition);
        space.AddSymbol("unknown", new PrimitiveTypeSymbol("unknown", PrimitiveType.UNKNOWN), tokenPosition);

        List<FileNode> nodes = [];
        foreach ((string name, string content) in files) {
            source = name;
            code = content;

            tokenPosition.source = source;
            index = 0;
            indexPosition = new(1, 1);
            NextChar();
            NextToken();
            nodes.Add(ParseRoot());
        }

        foreach (FileNode node in nodes) {
            node.AddItemSymbol(this);
        }
        foreach (FileNode node in nodes) {
            node.AddImplSymbol(this);
        }
        foreach (FileNode node in nodes) {
            node.SetSemantic(this);
        }

        this.files.Clear();
        return [.. nodes];
    }
}
