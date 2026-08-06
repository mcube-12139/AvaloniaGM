using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Symbols {
    internal class VariableSymbol(
        string name,
        UndertaleInstruction.VariableType variableType,
        UndertaleVariable variable,
        IType type
    ) : IValueSymbol {
        ITypeSymbol ISymbol.AsType(TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_TYPE, [name], position);
        }

        IValueSymbol ISymbol.AsValue(TextPosition position, Generator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        IType IValueSymbol.GetValueType(TextPosition position, Generator generator) {
            return type;
        }

        void IValueSymbol.Load(TextPosition position, Generator generator) {
            generator.Load(variable, variableType);
        }

        void IValueSymbol.Store(TextPosition position, Generator generator) {
            generator.Store(variable, variableType);
        }
    }
}
