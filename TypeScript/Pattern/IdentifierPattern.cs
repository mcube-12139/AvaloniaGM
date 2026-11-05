using TypeScript.Expression;
using TypeScript.GenerateContext;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Pattern;

public class IdentifierPattern(SourcePosition position, string name) : IPattern {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    ISymbol? symbol;

    public ISymbol GetSymbol() => symbol!;

    void IPattern.AddLocal(IType type, bool initialized, Compiler compiler) {
        symbol = new LocalSymbol(name, type, initialized);
        compiler.AddSymbol(name, symbol, position);
    }

    void IPattern.AddParameter(IType type, Compiler compiler) {
        symbol = new ParameterSymbol(name, type);
        compiler.AddSymbol(name, symbol, position);
    }

    void IPattern.GenerateLocal(IExpression? initializer, StatementContext context) {
        var localSymbol = (LocalSymbol)symbol!;
        localSymbol.index = context.GetLocalCount();
        context.AddLocalType(localSymbol.type.ToIntermediate());
        if (initializer != null) {
            ExpressionHelp.Generate(initializer, context);
            ((IValueSymbol)localSymbol).GenerateStore(position, context);
        }
    }

    void IPattern.GenerateParameter(IType type, ParameterContext context) {
        Intermediate.Parameter interParameter = new(name, type.ToIntermediate());
        if (symbol != null) {
            ((ParameterSymbol)symbol).interParameter = interParameter;
        }
        context.AddParameter(interParameter);
    }
}
