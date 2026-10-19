using TypeScript.ExceptionSpace;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Expression;

public class PathExpression(SourcePosition position, string[] segments, bool fromTop) : IPlaceExpression {
    public readonly SourcePosition position = position;
    public readonly string[] segments = segments;
    public readonly bool fromTop = fromTop;

    IType? type;
    IValueSymbol? symbol;

    SourcePosition IExpression.GetPosition() => position;

    void SetSymbolAndType(Compiler compiler) {
        ISymbol nowSymbol;
        if (!fromTop) {
            nowSymbol = compiler.GetSymbol(segments[0], position);
        } else {
            nowSymbol = compiler.GetTopSymbol(segments[0], position);
        }

        for (int i = 1; i != segments.Length; ++i) {
            nowSymbol = nowSymbol.GetNameSpace(position).GetSymbol(segments[i]) ?? throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [segments[i]], position);
        }
        symbol = nowSymbol.AsValue(position);
        type = symbol.LazyGetValueType(position, compiler);
    }

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        SetSymbolAndType(compiler);

        if (symbol is LocalSymbol localSymbol) {
            if (!localSymbol.initialized) {
                throw new SemanticException(SemanticErrorType.MAY_NOT_INITIALIZED, [symbol.GetName()], position);
            }
        }
    }

    IType IExpression.GetResultType() => type!;

    public IValueSymbol GetSymbol() => symbol!;

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        SetSymbolAndType(compiler);

        if (symbol is LocalSymbol localSymbol) {
            if (!localSymbol.initialized) {
                localSymbol.initialized = true;
                compiler.AddInitializedLocal(localSymbol);
            }
        }
    }
}
