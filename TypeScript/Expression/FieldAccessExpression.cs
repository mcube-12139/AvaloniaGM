using TypeScript.Field;
using TypeScript.Type;

namespace TypeScript.Expression;

public class FieldAccessExpression(SourcePosition position, IExpression accessed, string name) : IPlaceExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression accessed = accessed;
    public readonly string name = name;

    IField? field;
    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IField GetField() => field!;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        accessed.SetSemantic(null, compiler);
        field = accessed.GetResultType().GetField(name, position);
        type = field.GetFieldType();
    }

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        accessed.SetSemantic(null, compiler);
        field = accessed.GetResultType().GetField(name, position);
        field.EnsureCanSet(position);
        type = field.GetFieldType();
    }
}
