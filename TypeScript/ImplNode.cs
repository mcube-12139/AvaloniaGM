using TypeScript.ExceptionSpace;
using TypeScript.Item;
using TypeScript.Symbol;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript;

// implementations 既没有符号，又要在 Item 添加符号后、分析语义前改别人的符号（对应的类型），所以不是 Item。
public class ImplNode(SourcePosition position, ITypeNode typeNode, Path? regardedTypeNode, FunctionItem[] functions) {
    public readonly SourcePosition position = position;
    public readonly ITypeNode typeNode = typeNode;
    public readonly Path? regardedTypeNode = regardedTypeNode;
    public readonly FunctionItem[] functions = functions;

    IType? implementedType;
    IType? regardedType;
    public FunctionSymbol[]? methodTable;

    public IType GetImplementedType() => implementedType!;
    public IType GetRegardedType() => regardedType!;

    internal void AddSymbol(Compiler compiler) {
        typeNode.SetSemantic(compiler);
        implementedType = typeNode.GetSelfType(compiler);

        if (regardedTypeNode == null) {
            implementedType.AddFunctions(functions, position, compiler);
        } else {
            ((ITypeNode)regardedTypeNode).SetSemantic(compiler);
            regardedType = regardedTypeNode.GetSelfType(compiler);
            implementedType.AddImpl(regardedTypeNode.GetSymbol(), functions, position, compiler);
        }
    }

    internal void SetSemantic(Compiler compiler) {
        compiler.EnterThisType(typeNode.GetSelfType(compiler));
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }

        if (regardedTypeNode != null) {
            // 检查是否实现了该概念
            ISymbol symbol = regardedTypeNode.GetSymbol();
            Dictionary<string, ConceptFunctionSymbol> conceptFunctions = symbol.GetConceptFunctions(position);
            HashSet<string> conceptFunctionNames = [.. conceptFunctions.Keys];

            methodTable = new FunctionSymbol[functions.Length];
            foreach (FunctionItem funItem in functions) {
                if (!conceptFunctions.TryGetValue(funItem.name, out ConceptFunctionSymbol? conceptFun)) {
                    throw new SemanticException(SemanticErrorType.CONCEPT_FUNCTION_NOT_EXIST, [symbol.GetName(), funItem.name], funItem.position);
                }

                FunctionSymbol funSymbol = funItem.GetSymbol();
                conceptFun.EnsureSatisfySelf(funSymbol, funItem.position, compiler);
                methodTable[conceptFun.index] = funSymbol;

                conceptFunctionNames.Remove(funItem.name);
            }

            if (conceptFunctionNames.Count != 0) {
                throw new SemanticException(SemanticErrorType.CONCEPT_NOT_IMPLEMENTED, [string.Join(", ", conceptFunctionNames)], position);
            }

            implementedType!.SetImplMethods(regardedType!, methodTable, position);
        }
        compiler.LeaveThisType();
    }
}
