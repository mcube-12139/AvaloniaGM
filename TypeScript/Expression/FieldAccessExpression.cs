using TypeScript.Type;

namespace TypeScript.Expression;

public class FieldAccessExpression(SourcePosition position, IExpression accessed, string name) : IPlaceExpression {
    public readonly SourcePosition position = position;
    public readonly IExpression accessed = accessed;
    public readonly string name = name;

    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        accessed.SetSemantic(null, compiler);
        type = accessed.GetResultType().GetFieldAccessResultType(name, position);
    }

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        accessed.SetSemantic(null, compiler);
        type = accessed.GetResultType().GetFieldAccessResultType(name, position);
    }
}
