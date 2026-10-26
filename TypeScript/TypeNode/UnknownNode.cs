using TypeScript.Type;

namespace TypeScript.TypeNode;

public class UnknownNode : ITypeNode {
    public readonly SourcePosition position;

    internal UnknownNode(SourcePosition position) {
        this.position = position;
    }

    IType ITypeNode.GetSelfType() => PrimitiveType.UNKNOWN;

    void ITypeNode.SetSemantic(Compiler compiler) {
        // 无事发生
    }
}
