using TypeScript.ExceptionSpace;
using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;

namespace TypeScript.Item;

public class FunctionItem(SourcePosition position, string name, ThisParameter? thisParameter, IParameter[] parameters, ITypeNode? resultNode, BlockStatement? body) : IItem {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly ThisParameter? thisParameter = thisParameter;
    public readonly IParameter[] parameters = parameters;
    public readonly ITypeNode? resultNode = resultNode;
    public readonly BlockStatement? body = body;

    readonly bool method = thisParameter != null;
    FunctionSymbol? symbol;
    FunctionType? functionType;

    public FunctionSymbol GetSymbol() => symbol!;

    void IItem.AddSymbol(Compiler compiler) => AddSymbol(compiler);

    internal void AddSymbol(Compiler compiler) {
        if (body == null && !compiler.inInterface) {
            throw new SemanticException(SemanticErrorType.MISSING_FUNCTION_BODY, [], position);
        }

        symbol = FunctionSymbol.NewUnresolved(name, method, this, null);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    internal void SetSemantic(Compiler compiler) {
        ResolveType(compiler);

        IType resultType = functionType!.resultType;

        compiler.EnterFunction(resultType);

        thisParameter?.AddSymbol(compiler);
        foreach (IParameter parameter in parameters) {
            parameter.AddSymbol(compiler);
        }
        body?.SetSemantic(compiler);

        if (resultType != TupleType.EMPTY && !compiler.GetReturned()) {
            // 返回类型非()必须显式返回
            throw new SemanticException(SemanticErrorType.MAY_NOT_RETURN, [name], position);
        }

        compiler.LeaveFunction();
    }

    internal FunctionType ResolveType(Compiler compiler) {
        if (functionType == null) {
            thisParameter?.SetSemantic(compiler);
            foreach (IParameter parameter in parameters) {
                parameter.SetTypeSemantic(compiler);
            }
            resultNode?.SetSemantic(compiler);

            IType[] parameterTypes;
            if (method) {
                parameterTypes = [compiler.GetThisType()!, .. parameters.Select(parameter => parameter.GetParameterType())];
            } else {
                parameterTypes = [.. parameters.Select(parameter => parameter.GetParameterType())];
            }

            IType resultType = resultNode?.GetSelfType() ?? TupleType.EMPTY;

            functionType = FunctionType.Monomorphize(parameterTypes, resultType);
        }

        return functionType;
    }
}
