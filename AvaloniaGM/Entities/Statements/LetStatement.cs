using AvaloniaGM.Entities.Expressions;
using AvaloniaGM.Entities.Patterns;
using AvaloniaGM.Entities.TypeNodes;
using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Statements {
    internal class LetStatement(TextPosition position, IPattern pattern, ITypeNode? type, IExpression? initializer) : IStatement {
        void IStatement.Execute(TypeScriptGenerator generator) {
            pattern.AddVariable(type, initializer, generator);
        }
    }
}
