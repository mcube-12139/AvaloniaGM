using TypeScript.GenericArgumentNode;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public interface ITypeNode: IGenericArgumentNode {
    internal IType GetSelfType(Compiler compiler);
}
