using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.TypeNodes;
using AvaloniaGM.TypeScript.Types;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Expressions {
    internal class ArrayExpression(TextPosition position, IExpression[] elements, ITypeNode? typeNode) : IExpression {
        IType? type;
        
        IPlaceExpression IExpression.AsPlace(Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_PLACE, [], position);
        }

        void IExpression.Call(Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.Evaluate(Generator generator) {
            UndertaleVariable variable = EvaluateToVariable("arr", generator);
            generator.Load(variable, UndertaleInstruction.VariableType.Normal);
        }

        public IType GetResultType(Generator generator) {
            if (type == null) {
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
                    elementType = typeNode!.GetSharkType(generator);
                }

                type = new ArrayType(elementType);
            }

            return type;
        }

        void IExpression.GetIndex(IExpression index, Generator generator) {
            UndertaleVariable variable = EvaluateToVariable("indexed", generator);

            GetResultType(generator);
            type!.GetIndex(variable, index, position, generator);
        }

        UndertaleVariable EvaluateToVariable(string name, Generator generator) {
            UndertaleVariable variable = generator.AddLocalVariable(name);

            short elementIndex = 0;
            foreach (IExpression element in elements) {
                element.Evaluate(generator);

                generator.PushInt16((short)UndertaleInstruction.InstanceType.Local);
                generator.PushInt16(elementIndex);
                ++elementIndex;

                generator.Store(variable, UndertaleInstruction.VariableType.Array);
            }

            return variable;
        }

        UndertaleVariable IExpression.GetIndexDuplicate(IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }

        void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
