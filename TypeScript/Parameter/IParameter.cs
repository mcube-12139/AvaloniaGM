using TypeScript.Type;

namespace TypeScript.Parameter;

internal interface IParameter {
    IType GetSharkType(Compiler compiler);
    void SetSemantic(Compiler compiler);
}
