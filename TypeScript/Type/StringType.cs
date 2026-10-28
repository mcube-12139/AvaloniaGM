using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.GenericArgument;

namespace TypeScript.Type;

public class StringType : IType {
    string IGenericArgument.GetAppearance() => "string";
    static readonly Dictionary<string, IField> fields = new() {
        { "length", new StringLengthField() }
    };

    IField IType.GetField(string name, SourcePosition position) {
        if (fields.TryGetValue(name, out IField? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [name], position);
    }

    IType IType.GetAddResultType(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string + {rightType.GetAppearance()}"], position);
        }
        return this;
    }

    void IType.EnsureCanGreater(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string > {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanGreaterEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string >= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLess(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string < {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanLessEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string <= {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string == {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanNotEqual(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string != {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string = {rightType.GetAppearance()}"], position);
        }
    }

    void IType.EnsureCanAddAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (rightType != this) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"string += {rightType.GetAppearance()}"], position);
        }
    }
}
