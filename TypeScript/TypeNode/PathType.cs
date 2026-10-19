using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public class PathType(SourcePosition position, bool fromGlobal, string[] segments) : ITypeNode {
    public readonly SourcePosition position = position;
    public readonly bool fromGlobal = fromGlobal;
    public readonly string[] segments = segments;

    ITypeSymbol? symbol;

    public ITypeSymbol GetSymbol() => symbol!;

    IType ITypeNode.GetSelfType() {
        return symbol!.GetSelfType();
    }

    IType ITypeNode.LazyGetSelfType(Compiler compiler) {
        if (symbol == null) {
            SetSemantic(compiler);
        }
        return symbol!.GetSelfType();
    }

    void ITypeNode.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    void SetSemantic(Compiler compiler) {
        symbol ??= compiler.GetSymbol(segments[0], position).AsType(position, compiler);
    }
}
