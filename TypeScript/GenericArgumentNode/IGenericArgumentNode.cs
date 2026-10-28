using TypeScript.GenericArgument;

namespace TypeScript.GenericArgumentNode;

public interface IGenericArgumentNode {
    internal void SetSemantic(Compiler compiler);
    IGenericArgument GetArgument(Compiler compiler);
}
