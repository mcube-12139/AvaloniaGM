using TypeScript.Type;

namespace TypeScript.Parameter;

public interface IParameter {
    internal void SetSemantic(Compiler compiler);
    internal IType LazyGetParameterType(Compiler compiler);
    IType GetParameterType();
}
