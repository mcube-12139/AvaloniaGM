using TypeScript.ExceptionSpace;
using TypeScript.Item;
using TypeScript.NameSpace;
using TypeScript.Symbol;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript;

// implementations 既没有符号，又要在 Item 添加符号后、分析语义前改别人的符号（对应的类型），所以不能当作 Item 处理。
public class ImplNode {
    public readonly SourcePosition position;
    public readonly ITypeNode typeNode;
    public readonly Path? conceptNode;
    public readonly FunctionItem[] functions;

    IType? implementedType;
    IConceptSymbol? conceptSymbol;
    SymbolSpace? parameterSpace;

    internal ImplNode(SourcePosition position, ITypeNode typeNode, Path? conceptNode, FunctionItem[] functions) {
        this.position = position;
        this.typeNode = typeNode;
        this.conceptNode = conceptNode;
        this.functions = functions;
    }

    public IType GetImplementedType() => implementedType!;
    public IConceptSymbol? GetConceptSymbol() => conceptSymbol;

    internal void AddImpl(Compiler compiler) {
        typeNode.SetSemantic(compiler);
        implementedType = typeNode.GetSelfType(compiler);
        if (conceptNode == null) {
            foreach (FunctionItem funItem in functions) {
                implementedType.AddInherentImpl(funItem.CreateSymbol(compiler), position);
            }
        } else {
            ((ITypeNode)conceptNode).SetSemantic(compiler);
            conceptSymbol = conceptNode.GetSymbol().AsConcept(position);
            implementedType.AddConceptImpl(conceptSymbol, functions.Select(funItem => funItem.CreateSymbol(compiler)), position);
        }
    }

    internal void SetSemantic(Compiler compiler) {
        parameterSpace = compiler.EnterOrNewSymbolSpace(parameterSpace);
        compiler.thisTypeSymbol = new ThisTypeSymbol(implementedType!);

        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }

        if (conceptSymbol != null) {
            // 检查是否实现了该概念
            conceptSymbol.SetThisType(implementedType!, compiler);

            Dictionary<string, ConceptFunction> conceptFunctions = conceptSymbol.GetFunctions(compiler);
            HashSet<string> conceptFunctionNames = [.. conceptFunctions.Keys];

            foreach (FunctionItem funItem in functions) {
                if (!conceptFunctions.TryGetValue(funItem.name, out ConceptFunction? conceptFun)) {
                    throw new SemanticException(SemanticErrorType.CONCEPT_FUNCTION_NOT_EXIST, [conceptSymbol.GetName(), funItem.name], funItem.position);
                }

                if (funItem.symbol != null) {
                    conceptFun.EnsureSatisfy(funItem.symbol, funItem.position, compiler);
                } else {
                    throw new NotImplementedException();
                }

                conceptFunctionNames.Remove(funItem.name);
            }

            if (conceptFunctionNames.Count != 0) {
                throw new SemanticException(SemanticErrorType.CONCEPT_NOT_IMPLEMENTED, [string.Join(", ", conceptFunctionNames)], position);
            }

        }

        compiler.thisTypeSymbol = null;
        compiler.LeaveNameSpace();
    }
}
