using TypeScript.GenericArgumentNode;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public interface ITypeNode: IGenericArgumentNode {
    internal void SetSemantic(Compiler compiler);
    IType GetSelfType(Compiler compiler);
}
