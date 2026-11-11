using TypeScript.ExceptionSpace;
using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.FunctionBody;
using TypeScript.Intermediate;
using System.Collections.Immutable;

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
    internal ConceptFunctionSymbol? conceptSymbol;
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
        FunctionType funType = ResolveType(compiler);
        if (symbol != null && symbol.type == null) {
            symbol.type = funType;
        } else if (conceptSymbol != null && conceptSymbol.type == null) {
            conceptSymbol.type = funType;
        }

        IType resultType = funType.resultType;

        compiler.EnterFunction(resultType);

        thisParameter?.AddSymbol(compiler);
        foreach (IParameter parameter in parameters) {
            parameter.AddSymbol(compiler);
        }

        if (body != null) {
            body.SetSemantic(compiler);

            if (resultType != TupleType.EMPTY && !compiler.GetReturned()) {
                // 返回类型非()，必须显式返回
                throw new SemanticException(SemanticErrorType.MAY_NOT_RETURN, [name], position);
            }
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

            IType resultType = resultNode?.GetSelfType(compiler) ?? TupleType.EMPTY;

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
            resultType = resultNode?.GetSelfType(compiler) ?? TupleType.EMPTY;
        }

        return (parameterTypes, resultType);
    }

    void IItem.Generate(IItemContext context) {
        ParameterContext parameterContext = new();
        thisParameter?.Generate(parameterContext);
        foreach (IParameter parameter in parameters) {
            parameter.Generate(parameterContext);
        }
        ImmutableArray<Intermediate.Parameter> interParameters = [.. parameterContext.GetParameters()];

        Intermediate.Type.IType interResultType;
        if (symbol != null) {
            interResultType = symbol.type!.resultType.ToIntermediate();
        } else {
            interResultType = conceptSymbol!.type!.resultType.ToIntermediate();
        }

        if (symbol != null) {
            IFunctionBody funBody;
            if (hostName != null) {
                funBody = new HostBody(hostName);
            } else {
                StatementContext statementContext = new(context.GetProgramContext());
                ((IStatement)body!).Generate(statementContext);
                funBody = new GuestBody(statementContext.GetLocalTypes(), statementContext.GetInstructions());
            }
            Function result = new(name, interParameters, interResultType, funBody);
            context.AddFunction(result);
            symbol.ResolveInterFunction(result);
        } else {
            conceptSymbol!.ResolveVirtualFunction(new(name, interParameters, interResultType));
        }
    }
}
