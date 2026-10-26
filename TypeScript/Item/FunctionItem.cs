using TypeScript.ExceptionSpace;
using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;

namespace TypeScript.Item;

public class FunctionItem(
    SourcePosition position,
    string name,
    ThisParameter? thisParameter,
    IParameter[] parameters,
    ITypeNode? resultNode,
    BlockStatement? body,
    string? hostName
) : IItem {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly ThisParameter? thisParameter = thisParameter;
    public readonly IParameter[] parameters = parameters;
    public readonly ITypeNode? resultNode = resultNode;
    public readonly BlockStatement? body = body;
    public readonly string? hostName = hostName;

    readonly bool method = thisParameter != null;
    FunctionSymbol? symbol;
    FunctionType? functionType;
    ConceptFunctionSymbol? conceptSymbol;
    IType[]? parameterTypes;
    IType? resultType;

    public FunctionSymbol GetSymbol() => symbol!;

    void IItem.AddItemSymbol(Compiler compiler) {
        compiler.AddSymbol(name, CreateSymbol(compiler), position);
    }

    internal FunctionSymbol CreateSymbol(Compiler compiler) {
        if (body == null && hostName == null && !compiler.inInterface) {
            throw new SemanticException(SemanticErrorType.MISSING_FUNCTION_BODY, [], position);
        }

        symbol = new(name, compiler.space, method, this, hostName);
        return symbol;
    }

    internal void AddSymbolToSpace(NameSpace space, Compiler compiler) {
        space.AddSymbol(name, CreateSymbol(compiler), position);
    }

    internal ConceptFunctionSymbol CreateConceptSymbol(NameSpace space, uint index) {
        conceptSymbol = new(name, space, method, index, this);
        return conceptSymbol;
    }

    void IItem.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    internal void SetSemantic(Compiler compiler) {
        if (body != null) {
            FunctionType funType = ResolveType(compiler);

            IType resultType = funType.resultType;

            compiler.EnterFunction(resultType);

            thisParameter?.AddSymbol(compiler);
            foreach (IParameter parameter in parameters) {
                parameter.AddSymbol(compiler);
            }

            body.SetSemantic(compiler);

            if (resultType != TupleType.EMPTY && !compiler.GetReturned()) {
                // 返回类型非()，必须显式返回
                throw new SemanticException(SemanticErrorType.MAY_NOT_RETURN, [name], position);
            }

            compiler.LeaveFunction();
        }
    }

    internal void SetConceptSemantic(Compiler compiler) {
        // 无事发生
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

    internal (IType[], IType) ResolveConceptType(Compiler compiler) {
        if (parameterTypes == null || resultType == null) {
            foreach (IParameter parameter in parameters) {
                parameter.SetTypeSemantic(compiler);
            }
            resultNode?.SetSemantic(compiler);

            parameterTypes = [.. parameters.Select(parameter => parameter.GetParameterType())];
            resultType = resultNode?.GetSelfType() ?? TupleType.EMPTY;
        }

        return (parameterTypes, resultType);
    }
}
