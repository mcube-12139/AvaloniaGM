using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.TypeNodes;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Patterns {
    internal class IdentifierPattern(TextPosition position, string name) : IPattern {
        TextPosition position = position;

        void IPattern.AddVariable(ITypeNode? type, IExpression? initializer, Generator generator, TextPosition position) {
            UndertaleVariable variable = generator.AddLocalVariable(name);
            generator.AddSymbol(name, new VariableSymbol(UndertaleInstruction.VariableType.Normal, variable), position);

            if (initializer != null) {
                initializer.Evaluate(generator);
                generator.Pop(variable, UndertaleInstruction.DataType.Variable, UndertaleInstruction.VariableType.Normal);
            }
        }
    }
}
