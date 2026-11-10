using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Expression;

public class IndexExpression(SourcePosition position, IExpression indexed, IExpression index) : IPlaceExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression indexed = indexed;
    public readonly IExpression index = index;

    IType? type;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        indexed.SetSemantic(null, compiler);
        type = indexed.GetResultType().GetIndexResultType(index, position, compiler);
    }

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        indexed.SetSemantic(null, compiler);
        type = indexed.GetResultType().GetIndexResultType(index, position, compiler);
    }

    void IPlaceExpression.GenerateAssign(IExpression expression, StatementContext context) {
        ExpressionHelp.Generate(indexed, context);
        ExpressionHelp.Generate(index, context);
        ExpressionHelp.Generate(expression, context);
        indexed.GetResultType().GenerateSetIndex(position, context);
    }

    void IPlaceExpression.GenerateCompoundAssignStart(StatementContext context) {
        ExpressionHelp.Generate(indexed, context);
        ExpressionHelp.Generate(index, context);
        IType indexedType = indexed.GetResultType();
        context.AddInstruction(DuplicateInstruction.Instantiate([indexedType.ToIntermediate(), index.GetResultType().ToIntermediate()]));
        indexedType.GenerateGetIndex(position, context);
    }

    void IPlaceExpression.GenerateCompoundAssign(StatementContext context) {
        indexed.GetResultType().GenerateSetIndex(position, context);
    }

    void IExpression.Generate(StatementContext context) {
        ExpressionHelp.Generate(indexed, context);
        ExpressionHelp.Generate(index, context);
        indexed.GetResultType().GenerateGetIndex(position, context);
    }
}
