using System.Linq.Expressions;
using TypeScript.GenerateContext;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Expression;

public class MethodCallExpression(SourcePosition position, IExpression callee, string name, IExpression[] parameters) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression callee = callee;
    public readonly string name = name;
    public readonly IExpression[] parameters = parameters;

    ISymbol? symbol;
    IType? type;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        callee.SetSemantic(null, compiler);

        symbol = callee.GetResultType().GetAssociatedSymbol(name, position);
        type = symbol.GetMethodCallResultType(parameters, position, compiler);
    }

    void IExpression.Generate(StatementContext context) {
        ExpressionHelp.Generate(callee, context);
        foreach (IExpression parameter in parameters) {
            ExpressionHelp.Generate(parameter, context);
        }
        symbol!.GenerateCallMethod(position, context);
    }
}

