using TypeScript.ExceptionSpace;
using TypeScript.Item;
using TypeScript.Symbol;
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
            type.AddImpl(regardedType.GetSymbol(), functions, position, compiler);
        }
    }

    internal void SetSemantic(Compiler compiler) {
        compiler.EnterThisType(typeNode.GetSelfType());
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }

        if (regardedType != null) {
            // 检查是否实现了该概念
            ISymbol symbol = regardedType.GetSymbol();
            Dictionary<string, FunctionSymbol> conceptFunctions = symbol.GetConceptFunctions(position);
            HashSet<string> conceptFunctionNames = [.. conceptFunctions.Keys];
            foreach (FunctionItem funItem in functions) {
                if (!conceptFunctions.TryGetValue(funItem.name, out FunctionSymbol? conceptFun)) {
                    throw new SemanticException(SemanticErrorType.CONCEPT_FUNCTION_NOT_EXIST, [symbol.GetName(), funItem.name], funItem.position);
                }

                FunctionSymbol funSymbol = funItem.GetSymbol();
                if (funSymbol.method != conceptFun.method) {
                    throw new SemanticException(SemanticErrorType.NOT_IMPLEMENTING_CONCEPT_FUNCTION, [conceptFun.name], funItem.position);
                }

                FunctionType funType = funSymbol.LazyGetValueType(position, compiler);
                FunctionType conceptFunType = conceptFun.LazyGetValueType(position, compiler);
                if (
                    (!funSymbol.method && !funType.IsType(conceptFunType))
                    || (funSymbol.method && !funType.IsMethodType(conceptFunType))
                ) {
                    throw new SemanticException(SemanticErrorType.NOT_IMPLEMENTING_CONCEPT_FUNCTION, [conceptFun.name], funItem.position);
                }

                conceptFunctionNames.Remove(funItem.name);
            }

            if (conceptFunctionNames.Count != 0) {
                throw new SemanticException(SemanticErrorType.CONCEPT_NOT_IMPLEMENTED, [string.Join(", ", conceptFunctionNames)], position);
            }
        }
        compiler.LeaveThisType();
    }
}
