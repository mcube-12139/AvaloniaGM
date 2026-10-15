using TypeScript.Type;

namespace TypeScript.TypeNode;

public interface ITypeNode {
    void SetSemantic(Compiler compiler);
    IType GetSharkType(Compiler compiler);
}
