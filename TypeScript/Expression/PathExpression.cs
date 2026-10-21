using TypeScript.ExceptionSpace;
using TypeScript.PathSegment;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Expression;

public class PathExpression(SourcePosition position, IPathSegment[] segments, bool fromTop) : IPlaceExpression {
    public readonly SourcePosition position = position;
    public readonly IPathSegment[] segments = segments;
    public readonly bool fromTop = fromTop;

    IType? type;
    IValueSymbol? symbol;

    SourcePosition IExpression.GetPosition() => position;

    void SetSymbolAndType(Compiler compiler) {
        ISymbol nowSymbol;
        if (!fromTop) {
            nowSymbol = segments[0].GetSymbol(compiler);
        } else {
            nowSymbol = segments[0].GetSymbolInNameSpace(compiler.GetTopNameSpace());
        }

        for (int i = 1; i != segments.Length; ++i) {
            nowSymbol = segments[i].GetSymbolInNameSpace(nowSymbol.GetNameSpace(position));
        }
        symbol = nowSymbol.AsValue(position);
        type = symbol.LazyGetValueType(position, compiler);
    }

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        SetSymbolAndType(compiler);
        symbol!.EnsureCanLoad(position);
    }

    IType IExpression.GetResultType() => type!;

    public IValueSymbol GetSymbol() => symbol!;

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        SetSymbolAndType(compiler);
        symbol!.Store(position, compiler);
    }
}
