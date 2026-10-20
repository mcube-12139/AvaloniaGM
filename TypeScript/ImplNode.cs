using TypeScript.Item;
using TypeScript.TypeNode;

namespace TypeScript;

public class ImplNode(SourcePosition position, ITypeNode typeNode, PathType? regardedType, FunctionItem[] functions) {
    public readonly SourcePosition position = position;
    public readonly ITypeNode typeNode = typeNode;
    public readonly PathType? regardedType = regardedType;
    public readonly FunctionItem[] functions = functions;

    internal void AddSymbol(Compiler compiler) {
        typeNode.SetSemantic(compiler);
        regardedType?.SetSemantic(compiler);

        typeNode.AddImplSymbol(regardedType?.GetSelfType(), functions, compiler);

        foreach (FunctionItem funItem in functions) {
            if (funItem.thisParameter != null) {
                funItem.ResolveType(compiler);
                typeNode.GetSelfType().AddMethod(funItem.name, funItem.GetSymbol(), position);
            }
        }
    }

    internal void SetSemantic(Compiler compiler) {
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }
    }
}
