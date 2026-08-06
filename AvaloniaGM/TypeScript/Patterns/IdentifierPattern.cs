using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.TypeNodes;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Patterns {
    internal class IdentifierPattern(TextPosition position, string name) : IPattern {
        void IPattern.AddParameter(ITypeNode typeNode, Generator generator) {
            uint index = generator.NextParameterIndex();
            UndertaleVariable variable = generator.AddSelfVariable($"argument{index}", true);
            IType type = typeNode.GetSharkType(generator);
            generator.AddSymbol(name, new VariableSymbol(name, UndertaleInstruction.VariableType.Normal, variable, type), position);
        }

        void IPattern.AddVariable(ITypeNode? typeNode, IExpression? initializer, Generator generator, TextPosition position) {
            UndertaleVariable variable = generator.AddLocalVariable(name);

            IType type;
            if (initializer != null) {
                initializer.Evaluate(generator);
                type = initializer.GetResultType(generator);
                generator.Store(variable, UndertaleInstruction.VariableType.Normal);
            } else {
                throw new System.NotImplementedException();
            }

            generator.AddSymbol(name, new VariableSymbol(name, UndertaleInstruction.VariableType.Normal, variable, type), position);
        }
    }
}
