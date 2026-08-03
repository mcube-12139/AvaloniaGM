using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Symbols {
    internal class VariableSymbol(UndertaleInstruction.VariableType variableType, UndertaleVariable variable) : IValueSymbol {
        IValueSymbol ISymbol.AsValue(TextPosition position, Generator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IValueSymbol.Load(TextPosition position, Generator generator) {
            if (variable.InstanceType == UndertaleInstruction.InstanceType.Local) {
                generator.PushLocal(variable, UndertaleInstruction.DataType.Variable, variableType);
            } else {
                throw new System.NotImplementedException();
            }

            generator.PushType(UndertaleInstruction.DataType.Variable);
        }
    }
}
