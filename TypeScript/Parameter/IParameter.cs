using TypeScript.Type;

namespace TypeScript.Parameter;

public interface IParameter {
    IType GetSharkType(Compiler compiler);
    void SetSemantic(Compiler compiler);
}
