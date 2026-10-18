using TypeScript.Type;

namespace TypeScript.Parameter;

public class ThisParameter(TextPosition position) : IParameter {
    public readonly TextPosition position = position;

    IType IParameter.GetParameterType() {
        throw new NotImplementedException();
    }

    IType IParameter.LazyGetParameterType(Compiler compiler) {
        throw new NotImplementedException();
    }

    void IParameter.SetSemantic(Compiler compiler) {
        throw new NotImplementedException();
    }
}
