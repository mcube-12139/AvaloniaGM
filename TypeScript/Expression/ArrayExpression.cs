using TypeScript.ExceptionSpace;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Expression;

public class ArrayExpression(SourcePosition position, IExpression[] elements) : IExpression {
    internal readonly ExpressionHead head = new(position);
    public readonly IExpression[] elements = elements;

    ArrayType? type;

    ExpressionHead IExpression.GetHead() => head;

    public IType GetResultType() => type!;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        if (elements.Length != 0) {
            elements[0].SetSemantic(null, compiler);
            IType firstType = elements[0].GetResultType();
            for (int i = 1; i != elements.Length; ++i) {
                IExpression expression = elements[i];
                expression.SetSemantic(firstType, compiler);
                expression.SetTargetType(firstType);
            }

            type = ArrayType.Monomorphize(firstType);
        } else {
            if (target == null) {
                throw new SemanticException(SemanticErrorType.UNKNOWN_TYPE, [], position);
            }
            if (target is ArrayType arrayType) {
                type = arrayType;
            }
            throw new SemanticException(SemanticErrorType.WRONG_TYPE, ["[]", target.GetAppearance()], position);
        }
    }

    void IExpression.Generate(StatementContext context) {
        foreach (IExpression element in elements) {
            ExpressionHelp.Generate(element, context);
        }
        context.AddInstruction(new NewArrayInstruction(type!.elementType.ToIntermediate(), elements.Length));
    }
}

