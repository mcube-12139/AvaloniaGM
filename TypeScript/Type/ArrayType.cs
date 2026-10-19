using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class ArrayType(IType elementType): IType {
    public readonly IType elementType = elementType;

    public string GetAppearance() {
        return $"{elementType.GetAppearance()}[]";
    }

    bool IType.EqualType(IType other) {
        if (other is not ArrayType otherArray) {
            return false;
        }

        if (!elementType.EqualType(otherArray.elementType)) {
            return false;
        }

        return true;
    }

    IType IType.GetIndexResultType(IExpression index, SourcePosition position, Compiler compiler) {
        index.SetSemantic(PrimitiveType.INTEGER, compiler);
        IType indexType = index.GetResultType();
        if (!TypeHelp.IsType(indexType, PrimitiveType.INTEGER)) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()}[{indexType.GetAppearance()}]"], position);
        }

        return elementType;
    }

    void IType.EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (TypeHelp.IsType(rightType, this)) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (TypeHelp.IsType(rightType, this)) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (TypeHelp.IsType(rightType, this)) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} = {rightType.GetAppearance()}"], position);
        }
    }
}
