using TypeScript.ExceptionSpace;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Expression;

public class PathExpression(TextPosition position, string[] segments) : IPlaceExpression {
    public readonly TextPosition position = position;
    public readonly string[] segments = segments;

    IType? type;
    IValueSymbol? symbol;

    TextPosition IExpression.GetPosition() => position;

    void SetSymbolAndType(Compiler compiler) {
        symbol = compiler.GetSymbol(segments[0], position).AsValue(position, compiler);
        type = symbol.LazyGetValueType(position, compiler);
    }

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        SetSymbolAndType(compiler);

        if (symbol is LocalSymbol localSymbol) {
            if (!localSymbol.initialized) {
                throw compiler.SemanticError(SemanticErrorType.MAY_NOT_INITIALIZED, [symbol.GetName()], position);
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
