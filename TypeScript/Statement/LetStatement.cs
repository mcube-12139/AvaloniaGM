using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Pattern;
using TypeScript.TypeNode;
using TypeScript.Type;

namespace TypeScript.Statement;

public class LetStatement(TextPosition position, IPattern pattern, ITypeNode? type, IExpression? initializer) : IStatement {
    public readonly TextPosition position = position;
    public readonly IPattern pattern = pattern;
    public readonly ITypeNode? type = type;
    public readonly IExpression? initializer = initializer;


    void IStatement.SetSemantic(Compiler compiler) {
        IType definedType;

        if (type == null) {
            initializer!.SetSemantic(null, compiler);
            definedType = initializer.GetResultType();
        } else {
            type.SetSemantic(compiler);
            definedType = type.GetSharkType();

            if (initializer != null) {
                initializer.SetSemantic(definedType, compiler);
                IType initializerType = initializer.GetResultType();
                if (!initializerType.IsType(definedType)) {
                    throw compiler.SemanticError(SemanticErrorType.WRONG_TYPE, [initializerType.GetAppearance(), definedType.GetAppearance()], position);
                }
            }
        }

        pattern.AddSymbol(definedType, compiler);
    }
}
