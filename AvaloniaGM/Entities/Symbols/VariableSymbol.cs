using AvaloniaGM.Services;
using UndertaleModLib.Models;

namespace AvaloniaGM.Entities.Symbols {
    internal class VariableSymbol(UndertaleInstruction.VariableType variableType, UndertaleVariable variable) : IValueSymbol {
        IValueSymbol ISymbol.AsValue(TextPosition position, TypeScriptGenerator generator) {
            return this;
        }

        void IValueSymbol.Call(TextPosition position, TypeScriptGenerator generator) {
            throw new System.NotImplementedException();
        }

        void IValueSymbol.Load(TextPosition position, TypeScriptGenerator generator) {
            if (variable.InstanceType == UndertaleInstruction.InstanceType.Local) {
                generator.PushLocal(variable, UndertaleInstruction.DataType.Variable, variableType);
            } else {
                throw new System.NotImplementedException();
            }

            generator.PushType(UndertaleInstruction.DataType.Variable);
        }
    }
}
