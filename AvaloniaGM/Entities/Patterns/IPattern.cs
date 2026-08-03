using AvaloniaGM.Entities.Expressions;
using AvaloniaGM.Entities.TypeNodes;
using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Patterns {
    internal interface IPattern {
        void AddVariable(ITypeNode? type, IExpression? initializer, TypeScriptGenerator generator);
    }
}
