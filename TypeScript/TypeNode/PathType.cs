using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public class PathType(TextPosition position, bool fromGlobal, string[] segments) : ITypeNode {
    public readonly TextPosition position = position;
    public readonly bool fromGlobal = fromGlobal;
    public readonly string[] segments = segments;

    ITypeSymbol? symbol;

    public ITypeSymbol GetSymbol() => symbol!;

    IType ITypeNode.GetSelfType() {
        return symbol!.GetSharkType();
    }

    IType ITypeNode.LazyGetSelfType(Compiler compiler) {
        if (symbol == null) {
            SetSemantic(compiler);
        }
        return symbol!.GetSharkType();
    }

    void ITypeNode.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    void SetSemantic(Compiler compiler) {
        symbol ??= compiler.GetSymbol(segments[0], position).AsType(position, compiler);
    }
}
