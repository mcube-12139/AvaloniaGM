using TypeScript.Expression;
using TypeScript.Pattern;
using TypeScript.TypeNode;
using TypeScript.Type;

namespace TypeScript.Statement;

public class LetStatement(SourcePosition position, IPattern pattern, ITypeNode? type, IExpression? initializer) : IStatement {
    public readonly SourcePosition position = position;
    public readonly IPattern pattern = pattern;
    public readonly ITypeNode? type = type;
    public readonly IExpression? initializer = initializer;

    SourcePosition IStatement.GetPosition() => position;


    void IStatement.SetSemantic(Compiler compiler) {
        IType definedType;

        if (type == null) {
            initializer!.SetSemantic(null, compiler);
            definedType = initializer.GetResultType();
        } else {
            type.SetSemantic(compiler);
            definedType = type.GetSelfType(compiler);

            if (initializer != null) {
                initializer.SetSemantic(definedType, compiler);
                initializer.SetTargetType(definedType);
            }
        }

        pattern.AddLocal(definedType, initializer != null, compiler);
    }
}
