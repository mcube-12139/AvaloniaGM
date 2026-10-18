using TypeScript.ExceptionSpace;
using TypeScript.Expression;

namespace TypeScript.Type;

public class ArrayType(IType elementType): IType {
    public readonly IType elementType = elementType;

    public string GetAppearance() {
        return $"{elementType.GetAppearance()}[]";
    }

    bool IType.IsType(IType other) {
        if (other is not ArrayType otherArray) {
            return false;
        }

        if (!TypeHelp.IsType(elementType, otherArray.elementType)) {
            return false;
        }

        return true;
    }

    IType IType.GetIndexResultType(IExpression index, TextPosition position, Compiler compiler) {
        index.SetSemantic(PrimitiveType.INTEGER, compiler);
        IType indexType = index.GetResultType();
        if (!TypeHelp.IsType(indexType, PrimitiveType.INTEGER)) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()}[{indexType.GetAppearance()}]"], position);
        }

        return elementType;
    }

    void IType.EnsureCanEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (TypeHelp.IsType(rightType, this)) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (TypeHelp.IsType(rightType, this)) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, TextPosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (TypeHelp.IsType(rightType, this)) {
            throw compiler.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()} = {rightType.GetAppearance()}"], position);
        }
    }
}
