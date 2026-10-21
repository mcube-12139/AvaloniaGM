using TypeScript.Item;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript;

// implementations 既没有符号，又要在 Item 添加符号后、分析语义前改别人的符号（对应的类型），所以不是 Item。
public class ImplNode(SourcePosition position, ITypeNode typeNode, PathType? regardedType, FunctionItem[] functions) {
    public readonly SourcePosition position = position;
    public readonly ITypeNode typeNode = typeNode;
    public readonly PathType? regardedType = regardedType;
    public readonly FunctionItem[] functions = functions;

    internal void AddSymbol(Compiler compiler) {
        typeNode.SetSemantic(compiler);
        IType type = typeNode.GetSelfType();

        if (regardedType == null) {
            type.AddFunctions(functions, position, compiler);
        } else {
            regardedType.SetSemantic(compiler);
            type.AddImpl(regardedType.GetSelfType(), functions, position, compiler);
        }
    }

    internal void SetSemantic(Compiler compiler) {
        compiler.EnterThisType(typeNode.GetSelfType());
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }
        compiler.LeaveThisType();
    }
}
