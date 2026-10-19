using TypeScript.Pattern;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript.Parameter;

public class PatternParameter(SourcePosition position, IPattern pattern, ITypeNode typeNode) : IParameter {
    public readonly SourcePosition position = position;
    public readonly IPattern pattern = pattern;
    public readonly ITypeNode typeNode = typeNode;

    IType? type;

    IType IParameter.GetParameterType() => type!;

    void IParameter.SetTypeSemantic(Compiler compiler) {
        typeNode.SetSemantic(compiler);
        type = typeNode.GetSelfType();
    }

    void IParameter.AddSymbol(Compiler compiler) {
        pattern.AddParameter(type!, compiler);
    }
}
