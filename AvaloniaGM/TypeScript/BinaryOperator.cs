using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Tokens;
using System;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript {
    internal class BinaryOperator(int priority, Action<IExpression, IExpression, TextPosition, Generator> evaluate) {
        internal int priority = priority;
        internal Action<IExpression, IExpression, TextPosition, Generator> evaluate = evaluate;

        readonly static BinaryOperator ADD = new(12, (left, right, position, generator) => {
            left.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            right.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            generator.Add();
        });
        readonly static BinaryOperator SUBTRACT = new(12, (left, right, position, generator) => {
            left.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            right.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            generator.Subtract();
        });
        readonly static BinaryOperator MULTIPLY = new(13, (left, right, position, generator) => {
            left.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            right.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            generator.Multiply();
        });
        readonly static BinaryOperator DIVIDE = new(13, (left, right, position, generator) => {
            left.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            right.Evaluate(generator);
            if (generator.PeekType() == UndertaleInstruction.DataType.Boolean) {
                generator.Convert(UndertaleInstruction.DataType.Int32);
                generator.PushType(UndertaleInstruction.DataType.Int32);
            }
            generator.Divide();
        });

        internal static BinaryOperator? FromToken(int priority, FixedToken token) {
            BinaryOperator? result;

            if (priority < 12 && token == FixedToken.PLUS) {
                result = ADD;
            } else if (priority < 12 && token == FixedToken.MINUS) {
                result = SUBTRACT;
            } else if (priority < 13 && token == FixedToken.STAR) {
                result = MULTIPLY;
            } else if (priority < 13 && token == FixedToken.SLASH) {
                result = DIVIDE;
            } else {
                result = null;
            }

            return result;
        }
    }
}
