using TypeScript.ExceptionSpace;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript.Expression;

public class NewExpression(SourcePosition position, PathType pathType, NewExpression.Field[] fields): IExpression {
    public class Field(SourcePosition position, string name, IExpression value) {
        public readonly SourcePosition position = position;
        public readonly string name = name;
        public readonly IExpression value = value;

        ClassType.Field? classField;
        IType? valueType;

        public ClassType.Field GetField() => classField!;

        public IType GetValueType() => valueType!;

        internal void SetSemantic(ClassType type, Compiler compiler) {
            classField = type.InternalGetField(name, position);
            value.SetSemantic(classField.type, compiler);
            valueType = value.GetResultType();

            if (!TypeHelp.IsType(valueType, classField.type)) {
                throw new SemanticException(SemanticErrorType.WRONG_TYPE, [valueType.GetAppearance(), classField.type.GetAppearance()], position);
            }
        }
    }

    public readonly SourcePosition position = position;
    public readonly PathType pathType = pathType;
    public readonly Field[] fields = fields;

    ClassType? type;

    SourcePosition IExpression.GetPosition() => position;

    IType IExpression.GetResultType() => type!;

    public ClassType GetClassType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        pathType.SetSemantic(compiler);
        IType t = pathType.GetSelfType();
        if (t is not ClassType classType) {
            throw new SemanticException(SemanticErrorType.NOT_CLASS, [t.GetAppearance()], pathType.position);
        }
        type = classType;

        if (fields.Length != type.fields.Length) {
            throw new SemanticException(SemanticErrorType.WRONG_FIELD_COUNT, [fields.Length.ToString(), type.fields.Length.ToString()], position);
        }
        foreach (Field field in fields) {
            field.SetSemantic(type, compiler);
        }
    }
}
