using TypeScript.Expression;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class GenericType: IType {
    IConceptSymbol[] bounds;

    public IType? concreteType;

    internal GenericType(IConceptSymbol[] bounds, IType? concreteType) {
        this.bounds = bounds;
        this.concreteType = concreteType;
    }

    public string GetAppearance() {
        return string.Join(" & ", bounds);
    }

    IType IType.GetConcreteType() => concreteType!.GetConcreteType();

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
