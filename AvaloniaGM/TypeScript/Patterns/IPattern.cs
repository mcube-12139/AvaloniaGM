using AvaloniaGM.TypeScript;
using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.TypeNodes;

namespace AvaloniaGM.TypeScript.Patterns {
    internal interface IPattern {
        void AddVariable(ITypeNode? type, IExpression? initializer, Generator generator, TextPosition position);
    }
}
