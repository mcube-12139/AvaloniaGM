using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Patterns;
using AvaloniaGM.TypeScript.TypeNodes;

namespace AvaloniaGM.TypeScript.Statements {
    internal class LetStatement(TextPosition position, IPattern pattern, ITypeNode? type, IExpression? initializer) : IStatement {
        void IStatement.Execute(Generator generator) {
            pattern.AddVariable(type, initializer, generator, position);
        }
    }
}
