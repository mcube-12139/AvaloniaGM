using TypeScript.Type;

namespace TypeScript.Expression;

public class MethodCallExpression(SourcePosition position, IExpression callee, string name, IExpression[] parameters) : IExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression callee = callee;
    public readonly string name = name;
    public readonly IExpression[] parameters = parameters;

    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        callee.SetSemantic(null, compiler);
        FunctionType methodType = callee.GetResultType().GetMethodSymbol(name, position).GetFunctionType();
        methodType.EnsureCanCall(parameters, position, compiler);

        type = methodType.resultType;
    }
}
