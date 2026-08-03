using AvaloniaGM.Entities.Expressions;
using AvaloniaGM.Entities.Symbols;
using AvaloniaGM.Entities.TypeNodes;
using AvaloniaGM.Services;
using UndertaleModLib.Models;

namespace AvaloniaGM.Entities.Patterns {
    internal class IdentifierPattern(TextPosition position, string name) : IPattern {
        void IPattern.AddVariable(ITypeNode? type, IExpression? initializer, TypeScriptGenerator generator) {
            UndertaleVariable variable = generator.AddLocalVariable(name);
            generator.AddSymbol(name, new VariableSymbol(UndertaleInstruction.VariableType.Normal, variable), position);

            if (initializer != null) {
                initializer.Evaluate(generator);
                generator.Pop(variable, UndertaleInstruction.DataType.Variable, UndertaleInstruction.VariableType.Normal);
            }
        }
    }
}
