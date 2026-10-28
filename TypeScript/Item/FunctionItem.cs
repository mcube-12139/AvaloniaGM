using TypeScript.ExceptionSpace;
using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;
using TypeScript.GenericParameter;
using TypeScript.GenericArgument;

namespace TypeScript.Item;

public class FunctionItem : IItem {
    public readonly SourcePosition position;
    public readonly string name;
    public readonly IGenericParameter[]? genericParameters;
    public readonly ThisParameter? thisParameter;
    public readonly IParameter[] parameters;
    public readonly ITypeNode? resultNode;
    public readonly BlockStatement? body;
    public readonly string? hostName;

    NameSpace? parameterSpace;

    internal FunctionItem(
        SourcePosition position,
        string name,
        IGenericParameter[]? genericParameters,
        ThisParameter? thisParameter,
        IParameter[] parameters,
        ITypeNode? resultNode,
        BlockStatement? body,
        string? hostName
    ) {
        this.position = position;
        this.name = name;
        this.genericParameters = genericParameters;
        this.thisParameter = thisParameter;
        this.parameters = parameters;
        this.resultNode = resultNode;
        this.body = body;
        this.hostName = hostName;

        method = thisParameter != null;
    }

    readonly bool method;
    FunctionSymbol? symbol;
    public GenericFunctionSymbol? genericSymbol;
    ConceptFunctionSymbol? conceptSymbol;
    bool typeResolved = false;
    bool conceptTypeResolved = false;

    public FunctionSymbol GetSymbol() => symbol!;

    public bool IsGeneric() => genericParameters != null;

    void IItem.AddItemSymbol(Compiler compiler) {
        compiler.AddSymbol(name, CreateSymbol(compiler), position);
    }

    internal ISymbol CreateSymbol(Compiler compiler) {
        if (compiler.inInterface) {
            // 接口函数必须有 this，且无额外泛型参数
            if (!method) {
                throw new SemanticException(SemanticErrorType.MISSING_THIS, [], position);
            }
            if (genericParameters != null) {
                throw new SemanticException(SemanticErrorType.INVALID_GENERIC_PARAMETERS, [], position);
            }
        } else {
            // 非接口函数必须具体
            if (body == null && hostName == null) {
                throw new SemanticException(SemanticErrorType.MISSING_FUNCTION_BODY, [], position);
            }
        }

        if (genericParameters == null && !compiler.inInterface) {
            symbol = new(name, null, compiler.space, method, this, hostName);
            return symbol;
        } else {
            // 接口函数或带泛型参数的是泛型函数
            genericSymbol = new(name, compiler.space, method, this);
            return genericSymbol;
        }
    }

    internal void AddSymbolToSpace(NameSpace space, Compiler compiler) {
        space.AddSymbol(name, CreateSymbol(compiler), position);
    }

    internal ConceptFunctionSymbol CreateConceptSymbol(NameSpace space, uint index) {
        conceptSymbol = new(name, space, method, index, this);
        return conceptSymbol;
    }

    void IItem.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    void EnterParameterSpace(Compiler compiler) {
        if (parameterSpace == null) {
            parameterSpace = compiler.EnterNewNameSpace();
        } else {
            compiler.space = parameterSpace;
        }
    }

    internal void SetSemantic(Compiler compiler) {
        EnterParameterSpace(compiler);
        ResolveTypeInSpace(compiler);

        if (body != null) {
            IType resultType;
            if (symbol != null) {
                resultType = symbol.type!.resultType;
            } else {
                resultType = genericSymbol!.type!.resultType;
            }

            compiler.EnterFunction(resultType);

            thisParameter?.AddSymbol(compiler);
            foreach (IParameter parameter in parameters) {
                parameter.AddSymbol(compiler);
            }

            body.SetSemantic(compiler);

            // 返回类型非()，必须显式返回
            if (resultType != TupleType.EMPTY && !compiler.GetReturned()) {
                throw new SemanticException(SemanticErrorType.MAY_NOT_RETURN, [name], position);
            }

            compiler.LeaveFunction();
        }

        compiler.LeaveNameSpace();
    }

    internal void SetConceptSemantic(Compiler compiler) {
        // 无事发生
    }

    /// <summary>
    /// <para>在当前名字空间中分析函数类型。</para>
    /// <para>正常分析与提前分析时，前后对名字空间的处理不同。</para>
    /// </summary>
    /// <param name="compiler"></param>
    void ResolveTypeInSpace(Compiler compiler) {
        if (!typeResolved) {
            if (genericParameters != null) {
                List<IGenericParameterSymbol> genericParameterSymbols = [];
                foreach (IGenericParameter genericParameter in genericParameters) {
                    genericParameterSymbols.Add(genericParameter.AddSymbol(compiler));
                }
                genericSymbol!.parameterSymbols = [.. genericParameterSymbols];
            }

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

            if (symbol != null) {
                symbol.type = FunctionType.Instantiate(parameterTypes, resultType);
            } else {
                genericSymbol!.type = FunctionType.Instantiate(parameterTypes, resultType);
            }

            typeResolved = true;
        }
    }

    internal void ResolveType(Compiler compiler) {
        EnterParameterSpace(compiler);

        ResolveTypeInSpace(compiler);

        compiler.LeaveNameSpace();
    }

    internal void ResolveConceptType(Compiler compiler) {
        if (!conceptTypeResolved) {
            foreach (IParameter parameter in parameters) {
                parameter.SetTypeSemantic(compiler);
            }
            resultNode?.SetSemantic(compiler);

            conceptSymbol!.parameterTypes = [.. parameters.Select(parameter => parameter.GetParameterType())];
            conceptSymbol!.resultType = resultNode?.GetSelfType(compiler) ?? TupleType.EMPTY;

            conceptTypeResolved = true;
        }
    }
}
