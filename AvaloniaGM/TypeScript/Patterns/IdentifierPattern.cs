using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.TypeNodes;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Patterns {
    internal class IdentifierPattern(TextPosition position, string name) : IPattern {
        TextPosition position = position;

        void IPattern.AddVariable(ITypeNode? typeNode, IExpression? initializer, Generator generator, TextPosition position) {
            UndertaleVariable variable = generator.AddLocalVariable(name);

            IType type;
            if (initializer != null) {
                initializer.Evaluate(generator);
                type = initializer.GetResultType(generator);
                generator.Pop(variable, UndertaleInstruction.DataType.Variable, UndertaleInstruction.VariableType.Normal);
            } else {
                throw new System.NotImplementedException();
            }

            generator.AddSymbol(name, new VariableSymbol(UndertaleInstruction.VariableType.Normal, variable, type), position);
        }
    }
}
