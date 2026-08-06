using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class ArrayExpression(TextPosition position, IExpression[] elements) : IExpression {
        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            UndertaleVariable variable = generator.AddLocalVariable("arr");

            short index = 0;
            foreach (IExpression element in elements) {
                element.Evaluate(generator);

                generator.PushInt16((short)UndertaleInstruction.InstanceType.Local);
                generator.PushInt16(index);
                ++index;

                generator.Store(variable, UndertaleInstruction.VariableType.Array);
            }

            generator.Load(variable, UndertaleInstruction.VariableType.Normal);
        }

        public IType GetResultType(Generator generator) {
            IType elementType;
            if (elements.Length != 0) {
                elementType = elements[0].GetResultType(generator);
                for (int i = 1; i != elements.Length; ++i) {
                    IType nextType = elements[i].GetResultType(generator);
                    if (!nextType.IsType(elementType)) {
                        throw generator.SemanticError(SemanticErrorType.WRONG_TYPE, [nextType.GetAppearance(), elementType.GetAppearance()], position);
                    }
                }
            } else {
                elementType = PrimitiveType.NEVER;
            }

            return new ArrayType(elementType);
        }
    }
}
