using TypeScript.Type;

namespace TypeScript.TypeNode;

public interface ITypeNode {
    internal void SetSemantic(Compiler compiler);
    IType GetSelfType();
}
