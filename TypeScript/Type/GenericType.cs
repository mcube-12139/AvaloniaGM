using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class GenericType: IType {
    IConceptSymbol[] bounds;

    public IType? concreteType;

    internal GenericType(IConceptSymbol[] bounds) {
        this.bounds = bounds;
    }

    public string GetAppearance() {
        return string.Join(" & ", bounds);
    }

    IType IType.GetConcreteType() => concreteType!;

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
