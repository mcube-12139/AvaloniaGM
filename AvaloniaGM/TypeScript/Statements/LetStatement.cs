using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Patterns;
using AvaloniaGM.TypeScript.TypeNodes;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Statements {
    internal class LetStatement(TextPosition position, IPattern pattern, ITypeNode? type, IExpression? initializer) : IStatement {
        void IStatement.Execute(Generator generator) {
            if (type != null && initializer != null) {
                // 确定初始化式类型是定义类型
                IType definedType = type.GetSharkType(generator);
                IType initializerType = initializer.GetResultType(generator);
                if (!initializerType.IsType(definedType)) {
                    throw generator.SemanticError(SemanticErrorType.WRONG_TYPE, [initializerType.GetAppearance(), definedType.GetAppearance()], position);
                }
            }

            pattern.AddVariable(type, initializer, generator, position);
        }
    }
}
