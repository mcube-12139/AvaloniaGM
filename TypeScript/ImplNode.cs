using System.Collections.Immutable;
using TypeScript.ExceptionSpace;
using TypeScript.GenerateContext;
using TypeScript.Intermediate;
using TypeScript.Item;
using TypeScript.Symbol;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript;

// implementations 既没有符号，又要在 Item 添加符号后、分析语义前改别人的符号（对应的类型），所以不是 Item。
public class ImplNode(SourcePosition position, ITypeNode typeNode, PathType? regardedTypeNode, FunctionItem[] functions) {
    public readonly SourcePosition position = position;
    public readonly ITypeNode typeNode = typeNode;
    public readonly PathType? regardedTypeNode = regardedTypeNode;
    public readonly FunctionItem[] functions = functions;

    IType? implementedType;
    IConceptSymbol? conceptSymbol;
    Dictionary<ConceptFunctionSymbol, FunctionSymbol> implFunctions = [];

    internal void AddSymbol(Compiler compiler) {
        typeNode.SetSemantic(compiler);
        implementedType = typeNode.GetSelfType(compiler);

        if (regardedTypeNode == null) {
            implementedType.AddFunctions(functions, position, compiler);
        } else {
            regardedTypeNode.SetSemantic(compiler);
            conceptSymbol = regardedTypeNode.GetSymbol().AsConcept(position);
            implementedType.AddImpl(conceptSymbol, functions, position, compiler);
        }
    }

    internal void SetSemantic(Compiler compiler) {
        IType thisType = typeNode.GetSelfType(compiler);
        if (thisType is not ClassType) {
            throw new SemanticException(SemanticErrorType.NOT_CLASS, [thisType.GetAppearance()], position);
        }

        compiler.EnterThisType(thisType);
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }

        if (regardedTypeNode != null) {
            // 检查是否实现了该概念
            ISymbol symbol = regardedTypeNode.GetSymbol();
            Dictionary<string, ConceptFunctionSymbol> conceptFunctions = symbol.GetConceptFunctions(position);
            HashSet<string> conceptFunctionNames = [.. conceptFunctions.Keys];

            foreach (FunctionItem funItem in functions) {
                if (!conceptFunctions.TryGetValue(funItem.name, out ConceptFunctionSymbol? conceptFun)) {
                    throw new SemanticException(SemanticErrorType.CONCEPT_FUNCTION_NOT_EXIST, [symbol.GetName(), funItem.name], funItem.position);
                }

                FunctionSymbol funSymbol = funItem.GetSymbol();
                conceptFun.EnsureSatisfySelf(funSymbol, funItem.position, compiler);
                implFunctions[conceptFun] = funSymbol;

                conceptFunctionNames.Remove(funItem.name);
            }

            if (conceptFunctionNames.Count != 0) {
                throw new SemanticException(SemanticErrorType.CONCEPT_NOT_IMPLEMENTED, [string.Join(", ", conceptFunctionNames)], position);
            }
        }
        compiler.LeaveThisType();
    }

    internal void Generate(IItemContext context) {
        foreach (FunctionItem funItem in functions) {
            ((IItem)funItem).Generate(context);
        }
        if (conceptSymbol is InterfaceSymbol interfaceSymbol) {
            // 生成实现表
            context.AddImpl(
                (interfaceSymbol.type.intermediateType!, ((ClassType)implementedType!).intermediateClass),
                new(ImmutableDictionary.CreateRange(implFunctions.Select(
                    kv => KeyValuePair.Create(kv.Key.virtualFun!, kv.Value.intermediateFun!)
                )))
            );
        }
    }
}
