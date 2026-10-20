using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.TypeNode;

public interface ITypeNode {
    internal void SetSemantic(Compiler compiler);
    internal void AddImplSymbol(IType? regardedType, FunctionItem[] functions, Compiler compiler);
    IType GetSelfType();
}
