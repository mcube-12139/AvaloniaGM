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

    IType IParameter.LazyGetParameterType(Compiler compiler) {
        if (type == null) {
            typeNode.SetSemantic(compiler);
            type = typeNode.GetSelfType();
        }
        return type!;
    }

    void SetSemantic(Compiler compiler) {
        if (type == null) {
            typeNode.SetSemantic(compiler);
            type = typeNode.GetSelfType();
        }

        pattern.AddParameter(type, compiler);
    }

    void IParameter.SetSemantic(Compiler compiler) => SetSemantic(compiler);
}
