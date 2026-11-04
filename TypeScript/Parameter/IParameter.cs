using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Parameter;

public interface IParameter {
    internal void SetTypeSemantic(Compiler compiler);
    internal void AddSymbol(Compiler compiler);
    IType GetParameterType();
    internal void Generate(ParameterContext context);
}
