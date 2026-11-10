using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public class PathType(SourcePosition position, bool fromTop, string[] segments) : ITypeNode {
    public readonly SourcePosition position = position;
    public readonly bool fromTop = fromTop;
    public readonly string[] segments = segments;

    ISymbol? symbol;

    public ISymbol GetSymbol() => symbol!;

    IType ITypeNode.GetSelfType(Compiler compiler) => symbol!.AsType(position).LazyGetSelfType(position, compiler);

    void ITypeNode.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    internal void SetSemantic(Compiler compiler) {
        ISymbol nowSymbol;
        if (!fromTop) {
            nowSymbol = compiler.GetSymbol(segments[0], position);
        } else {
            nowSymbol = compiler.GetTopNameSpace().GetSymbol(segments[0], position);
        }

        for (int i = 1; i != segments.Length; ++i) {
            nowSymbol = nowSymbol.GetNameSpace(position).GetSymbol(segments[i], position);
        }
        symbol = nowSymbol;
    }
}
