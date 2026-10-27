using TypeScript.GenericArgument;

namespace TypeScript.GenericArgumentNode;

public interface IGenericArgumentNode {
    IGenericArgument GetArgument(Compiler compiler);
}
