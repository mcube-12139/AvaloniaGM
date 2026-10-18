using TypeScript.Type;

namespace TypeScript.TypeNode;

public interface ITypeNode {
    internal void SetSemantic(Compiler compiler);
    internal IType LazyGetSelfType(Compiler compiler);
    IType GetSelfType();
}
