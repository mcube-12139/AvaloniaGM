using TypeScript.Type;

namespace TypeScript.Parameter;

public class ThisParameter(SourcePosition position) : IParameter {
    public readonly SourcePosition position = position;

    void IParameter.AddSymbol(Compiler compiler) {
        throw new NotImplementedException();
    }

    IType IParameter.GetParameterType() {
        throw new NotImplementedException();
    }

    void IParameter.SetTypeSemantic(Compiler compiler) {
        throw new NotImplementedException();
    }
}
