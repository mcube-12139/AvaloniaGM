using TypeScript.Field;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Expression;

public class FieldAccessExpression(SourcePosition position, IExpression accessed, string name) : IPlaceExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression accessed = accessed;
    public readonly string name = name;

    IField? field;
    IType? type;

    ExpressionHead IExpression.GetHead() => head;

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

    void IPlaceExpression.GenerateAssign(IExpression expression, StatementContext context) {
        ExpressionHelp.Generate(accessed, context);
        ExpressionHelp.Generate(expression, context);
        field!.GenerateStore(context);
    }

    void IPlaceExpression.GenerateCompoundAssignStart(StatementContext context) {
        ExpressionHelp.Generate(accessed, context);
        context.AddInstruction(DuplicateInstruction.Instantiate([accessed.GetResultType().ToIntermediate()]));
        field!.GenerateLoad(context);
    }

    void IPlaceExpression.GenerateCompoundAssign(StatementContext context) {
        field!.GenerateStore(context);
    }

    void IExpression.Generate(StatementContext context) {
        ExpressionHelp.Generate(accessed, context);
        field!.GenerateLoad(context);
    }
}

