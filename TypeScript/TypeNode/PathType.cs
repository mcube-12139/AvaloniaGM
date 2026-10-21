using TypeScript.ExceptionSpace;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public class PathType(SourcePosition position, bool fromTop, string[] segments) : ITypeNode {
    public readonly SourcePosition position = position;
    public readonly bool fromTop = fromTop;
    public readonly string[] segments = segments;

    ITypeSymbol? symbol;
    IType? type;

    public ITypeSymbol GetSymbol() => symbol!;

    IType ITypeNode.GetSelfType() => type!;

    void ITypeNode.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    internal IType GetSelfType() => type!;

    internal void SetSemantic(Compiler compiler) {
        ISymbol nowSymbol;
        if (!fromTop) {
            nowSymbol = compiler.GetSymbol(segments[0], position);
        } else {
            throw new NotImplementedException();
        }

        for (int i = 1; i != segments.Length; ++i) {
            nowSymbol = nowSymbol.GetNameSpace(position).TryGetSymbol(segments[i]) ?? throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [segments[i]], position);
        }
        symbol = nowSymbol.AsType(position);
        type = symbol.LazyGetSelfType(position, compiler);
    }
}
