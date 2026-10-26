using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ConceptFunctionSymbol : ISymbol {
    public readonly string name;
    internal readonly NameSpace itemSpace;
    public readonly bool method;
    public readonly uint index;
    internal IType[]? parameterTypes;
    internal IType? resultType;
    readonly FunctionItem? item;

    internal ConceptFunctionSymbol(string name, NameSpace itemSpace, bool method, uint index, FunctionItem? item) {
        this.name = name;
        this.itemSpace = itemSpace;
        this.method = method;
        this.index = index;
        this.item = item;
    }

    string ISymbol.GetName() => name;

    public IType[] GetParameterTypes() => parameterTypes!;

    public IType GetResultType() => resultType!;

    internal void LazyGetValueType(Compiler compiler) {
        // 不能调用 SetSemantic，会无限递归
        if (parameterTypes == null && resultType == null) {
            compiler.PushNameSpace(itemSpace);
            (parameterTypes, resultType) = item!.ResolveConceptType(compiler);
            compiler.PopNameSpace();
        }
    }

    IType ISymbol.GetMethodCallResultType(IExpression[] parameterTypes, SourcePosition position, Compiler compiler) {
        if (!method) {
            throw new SemanticException(SemanticErrorType.NOT_METHOD, [name], position);
        }

        LazyGetValueType(compiler);

        if (parameterTypes.Length != this.parameterTypes!.Length) {
            throw new SemanticException(SemanticErrorType.WRONG_PARAMETER_COUNT, [parameterTypes.Length.ToString(), this.parameterTypes.Length.ToString()], position);
        }
        for (int i = 0; i != parameterTypes.Length; ++i) {
            IExpression parameter = parameterTypes[i];
            IType definedType = this.parameterTypes[i];
            parameter.SetSemantic(definedType, compiler);

            IType parameterType = parameter.GetResultType();
            if (!TypeHelp.IsType(parameterType, definedType)) {
                throw new SemanticException(SemanticErrorType.WRONG_TYPE, [parameterType.GetAppearance(), definedType.GetAppearance()], position);
            }
        }

        return resultType!;
    }

    internal void EnsureSatisfySelf(FunctionSymbol funSymbol, SourcePosition position, Compiler compiler) {
        if (funSymbol.method != method) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
        }

        FunctionType funType = funSymbol.LazyGetValueType(position, compiler);
        LazyGetValueType(compiler);

        if (!TypeHelp.IsType(resultType!, funType.resultType)) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
        }

        if (parameterTypes!.Length + 1 != funType.parameterTypes.Length) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
        }

        // 参数逆变
        for (int i = 0; i < parameterTypes.Length; ++i) {
            if (!TypeHelp.IsType(funType.parameterTypes[i + 1], parameterTypes[i])) {
                throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
            }
        }
    }
}
