using TypeScript.ExceptionSpace;
using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;
using TypeScript.GenericParameter;
using TypeScript.NameSpace;

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

    SymbolSpace? parameterSpace;
    readonly bool method;
    public FunctionSymbol? symbol;
    public GenericFunctionSymbol? genericSymbol;
    ConceptFunction? conceptFun;
    bool typeResolved = false;

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

    public bool IsGeneric() => genericParameters != null;

    void IItem.AddItemSymbol(Compiler compiler) {
        compiler.AddSymbol(name, CreateSymbol(compiler), position);
        parameterSpace = compiler.EnterOrNewSymbolSpace(parameterSpace);
        if (body != null) {
            ((IItem)body).AddItemSymbol(compiler);
        }
        compiler.LeaveNameSpace();
    }

    void IItem.AddImpl(Compiler compiler) {
        if (body != null) {
            parameterSpace = compiler.EnterOrNewSymbolSpace(parameterSpace);
            ((IItem)body).AddImpl(compiler);
            compiler.LeaveNameSpace();
        }
    }

    internal ISymbol CreateSymbol(Compiler compiler) {
        if (compiler.conceptContext == ConceptContext.INTERFACE) {
            // 接口函数必须有 this，且无额外泛型参数
            if (!method) {
                throw new SemanticException(SemanticErrorType.MISSING_THIS, [], position);
            }
            if (genericParameters != null) {
                throw new SemanticException(SemanticErrorType.INVALID_GENERIC_PARAMETERS, [], position);
            }
        } else if (compiler.conceptContext == ConceptContext.NONE) {
            // 非接口函数必须具体
            if (body == null && hostName == null) {
                throw new SemanticException(SemanticErrorType.MISSING_FUNCTION_BODY, [], position);
            }
        }

        if (genericParameters == null && compiler.conceptContext == ConceptContext.NONE) {
            symbol = new(name, null, compiler.GetSpaceState(), method, this, hostName);
            return symbol;
        } else {
            // 接口函数或带泛型参数的是泛型函数
            genericSymbol = new(name, compiler.GetSpaceState(), method, this);
            return genericSymbol;
        }
    }

    internal ConceptFunction CreateConceptFunction(Compiler compiler) {
        parameterSpace = compiler.EnterOrNewSymbolSpace(parameterSpace);
        conceptFun = new(name, method);
        compiler.LeaveNameSpace();
        return conceptFun;
    }

    void IItem.SetSemantic(Compiler compiler) => SetSemantic(compiler);

    internal void SetSemantic(Compiler compiler) {
        parameterSpace = compiler.EnterOrNewSymbolSpace(parameterSpace);
        ResolveTypeInSpace(compiler);

        if (body != null) {
            IType resultType;
            if (symbol != null) {
                resultType = symbol.type!.resultType;
            } else if (genericSymbol != null) {
                resultType = genericSymbol.type!.resultType;
            } else {
                resultType = conceptFun!.resultType!;
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
            } else if (genericSymbol != null) {
                genericSymbol.type = FunctionType.Instantiate(parameterTypes, resultType);
            } else {
                conceptFun!.parameterTypes = parameterTypes;
                conceptFun.resultType = resultType;
            }

            typeResolved = true;
        }
    }

    internal void ResolveType(Compiler compiler) {
        parameterSpace = compiler.EnterOrNewSymbolSpace(parameterSpace);

        ResolveTypeInSpace(compiler);

        compiler.LeaveNameSpace();
    }
}
