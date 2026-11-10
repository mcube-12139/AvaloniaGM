using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Type;

public class ArrayType: IType {
    static readonly Dictionary<IType, ArrayType> monomorphized = [];
    static readonly Dictionary<string, IField> fields = new() {
        { "length", new ArrayLengthField() }
    };

    public readonly IType elementType;

    readonly Intermediate.Type.ArrayType intermediateType;

    ArrayType(IType elementType) {
        this.elementType = elementType;

        intermediateType = new Intermediate.Type.ArrayType(elementType.ToIntermediate());
    }

    internal static ArrayType Monomorphize(IType elementType) {
        if (!monomorphized.TryGetValue(elementType, out ArrayType? result)) {
            result = new(elementType);
            monomorphized.Add(elementType, result);
        }

        return result;
    }

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

    Intermediate.Type.IType IType.ToIntermediate() => intermediateType;

    IType IType.GetIndexResultType(IExpression index, SourcePosition position, Compiler compiler) {
        index.SetSemantic(PrimitiveType.INTEGER, compiler);
        IType indexType = index.GetResultType();
        if (!TypeHelp.IsType(indexType, PrimitiveType.INTEGER)) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{GetAppearance()}[{indexType.GetAppearance()}]"], position);
        }

        return elementType;
    }

    IField IType.GetField(string name, SourcePosition position) {
        if (fields.TryGetValue(name, out IField? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [name], position);
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

    void IType.GenerateSetIndex(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(StoreElementInstruction.Instantiate(elementType.ToIntermediate()), position);
    }

    void IType.GenerateGetIndex(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(LoadElementInstruction.Instantiate(elementType.ToIntermediate()), position);
    }
}
