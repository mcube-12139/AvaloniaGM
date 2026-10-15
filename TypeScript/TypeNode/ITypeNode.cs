using TypeScript.Type;

namespace TypeScript.TypeNode;

internal interface ITypeNode {
    void SetSemantic(Compiler compiler);
    IType GetSharkType(Compiler compiler);
}
