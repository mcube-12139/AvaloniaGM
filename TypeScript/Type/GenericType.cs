using TypeScript.Expression;
using TypeScript.NameSpace;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class GenericType: IType {
    readonly TypeHead head;
    readonly IConceptSymbol[] bounds;

    public IType? concreteType;

    TypeHead IType.GetHead() => head;

    GenericType(IConceptSymbol[] bounds, IType? concreteType) {
        head = new();
        this.bounds = bounds;
        this.concreteType = concreteType;
    }

    /// <summary>
    /// 防止创建 This 时死递归
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    internal static GenericType Unresolved(IConceptSymbol[] bounds) {
        return new(bounds, null);
    }

    internal static GenericType WithBounds(IConceptSymbol[] bounds, SourcePosition position, Compiler compiler) {
        GenericType result = new(bounds, null);
        foreach (IConceptSymbol bound in bounds) {
            bound.SetThisType(result, compiler);

            SymbolSpace space = new(null);
            foreach ((string name, ConceptFunction fun) in bound.GetFunctions(compiler)) {
                IType[] parameterTypes = [.. fun.parameterTypes!.Select(type => type.TryGetConcreteType())];
                IType resultType = fun.resultType!.TryGetConcreteType();
                ConceptFunctionSymbol symbol = new(name, FunctionType.Instantiate(parameterTypes, resultType), result, bound);

                space.AddSymbol(name, symbol, position);
                result.head.space.AddSymbol(name, symbol, position);
            }
            result.head.impls.Add(bound, space);
        }

        return result;
    }

    internal void ResolveBounds(SourcePosition position, Compiler compiler) {
        foreach (IConceptSymbol bound in bounds) {
            SymbolSpace space = new(null);
            foreach ((string name, ConceptFunction fun) in bound.GetFunctions(compiler)) {
                ConceptFunctionSymbol symbol = new(name, FunctionType.Instantiate(fun.parameterTypes!, fun.resultType!), this, bound);

                space.AddSymbol(name, symbol, position);
                head.space.AddSymbol(name, symbol, position);
            }
            head.impls.Add(bound, space);
        }
    }

    public string GetAppearance() {
        return concreteType?.GetAppearance() ?? string.Join(" & ", bounds.Select(concept => concept.GetName()));
    }

    IType IType.GetConcreteType() => concreteType!.GetConcreteType();

    IType IType.TryGetConcreteType() {
        return concreteType?.TryGetConcreteType() ?? this;
    }

    void IType.InferGenericByArgument(IExpression argument, GenericInferContext context) {
        argument.SetSemantic(null, context.compiler);
        context.AddInferedType(this, argument.GetResultType());
    }

    void IType.InferGenericByTarget(IType? target, GenericInferContext context) {
        if (target != null) {
            context.AddInferedType(this, target);
        }
    }

    IType IType.GetAddResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(PrimitiveType.INTEGER, compiler);
        /*
        IType rightType = right.GetResultType();
        if (rightType != PrimitiveType.INTEGER) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
        }
        */
        return this;
    }
}
