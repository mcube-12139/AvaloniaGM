using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.TypeNodes;

namespace AvaloniaGM.TypeScript.Patterns {
    internal interface IPattern {
        void AddParameter(ITypeNode typeNode, Generator generator);
        void AddVariable(ITypeNode? typeNode, IExpression? initializer, Generator generator, TextPosition position);
    }
}
