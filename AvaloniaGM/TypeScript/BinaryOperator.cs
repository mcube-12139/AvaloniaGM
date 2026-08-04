using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using AvaloniaGM.TypeScript.Tokens;
using AvaloniaGM.TypeScript.Types;
using System;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript {
    internal class BinaryOperator(int priority, Action<IExpression, IExpression, TextPosition, Generator> evaluate) {
        internal int priority = priority;
        internal Action<IExpression, IExpression, TextPosition, Generator> evaluate = evaluate;

        readonly static BinaryOperator MULTIPLY = new(12, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Multiply(right, position, generator);
        });
        readonly static BinaryOperator DIVIDE = new(12, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator MODULO = new(12, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Modulo(right, position, generator);
        });
        readonly static BinaryOperator ADD = new(11, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Add(right, position, generator);
        });
        readonly static BinaryOperator SUBTRACT = new(11, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Subtract(right, position, generator);
        });
        readonly static BinaryOperator LEFT_SHIFT = new(10, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).LeftShift(right, position, generator);
        });
        readonly static BinaryOperator RIGHT_SHIFT = new(10, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).RightShift(right, position, generator);
        });
        readonly static BinaryOperator BIT_AND = new(9, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).BitAnd(right, position, generator);
        });
        readonly static BinaryOperator BIT_XOR = new(8, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).BitXor(right, position, generator);
        });
        readonly static BinaryOperator BIT_OR = new(7, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).BitOr(right, position, generator);
        });
        readonly static BinaryOperator GREATER = new(6, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Greater(right, position, generator);
        });
        readonly static BinaryOperator GREATER_EQUAL = new(6, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).GreaterEqual(right, position, generator);
        });
        readonly static BinaryOperator LESS = new(6, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Less(right, position, generator);
        });
        readonly static BinaryOperator LESS_EQUAL = new(6, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).LessEqual(right, position, generator);
        });
        readonly static BinaryOperator EQUAL = new(6, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Equal(right, position, generator);
        });
        readonly static BinaryOperator NOT_EQUAL = new(6, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).NotEqual(right, position, generator);
        });
        readonly static BinaryOperator AND = new(5, (left, right, position, generator) => {
            left.Evaluate(generator);
            IType leftType = left.GetResultType(generator);
            if (leftType != PrimitiveType.BOOLEAN) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{leftType.GetAppearance()} && ?"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Boolean);

            uint bfStart = generator.GetByteCount();
            UndertaleInstruction bfInstruction = generator.BranchFalse();

            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != PrimitiveType.BOOLEAN) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"boolean && {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Boolean);

            uint branchStart = generator.GetByteCount();
            UndertaleInstruction branchInstruction = generator.Branch();

            bfInstruction.JumpOffset = (int)(generator.GetByteCount() - bfStart) / 4;
            generator.PushBoolean(false);

            branchInstruction.JumpOffset = (int)(generator.GetByteCount() - branchStart) / 4;
        });
        readonly static BinaryOperator OR = new(4, (left, right, position, generator) => {
            left.Evaluate(generator);
            IType leftType = left.GetResultType(generator);
            if (leftType != PrimitiveType.BOOLEAN) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"{leftType.GetAppearance()} || ?"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Boolean);

            uint bfStart = generator.GetByteCount();
            UndertaleInstruction btInstruction = generator.BranchTrue();

            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != PrimitiveType.BOOLEAN) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"boolean || {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Boolean);

            uint branchStart = generator.GetByteCount();
            UndertaleInstruction branchInstruction = generator.Branch();

            btInstruction.JumpOffset = (int)(generator.GetByteCount() - bfStart) / 4;
            generator.PushBoolean(true);

            branchInstruction.JumpOffset = (int)(generator.GetByteCount() - branchStart) / 4;
        });
        readonly static BinaryOperator ASSIGN = new(2, (left, right, position, generator) => {
            left.AsPlace(generator).Assign(right, generator);
        });
        readonly static BinaryOperator ADD_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator SUBTRACT_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator MULTIPLY_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator DIVIDE_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator MODULO_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator BIT_AND_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator BIT_OR_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator BIT_XOR_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator LEFT_SHIFT_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });
        readonly static BinaryOperator RIGHT_SHIFT_ASSIGN = new(2, (left, right, position, generator) => {
            left.Evaluate(generator);
            left.GetResultType(generator).Divide(right, position, generator);
        });

        internal static BinaryOperator? FromToken(int priority, FixedToken token) {
            BinaryOperator? result;

            if (priority < 12 && token == FixedToken.STAR) {
                result = MULTIPLY;
            } else if (priority < 12 && token == FixedToken.SLASH) {
                result = DIVIDE;
            } else if (priority < 12 && token == FixedToken.PERCENT) {
                result = MODULO;
            } else if (priority < 11 && token == FixedToken.PLUS) {
                result = ADD;
            } else if (priority < 11 && token == FixedToken.MINUS) {
                result = SUBTRACT;
            } else if (priority < 10 && token == FixedToken.DOUBLE_LESS) {
                result = LEFT_SHIFT;
            } else if (priority < 10 && token == FixedToken.DOUBLE_GREATER) {
                result = RIGHT_SHIFT;
            } else if (priority < 9 && token == FixedToken.AND) {
                result = BIT_AND;
            } else if (priority < 8 && token == FixedToken.CARET) {
                result = BIT_XOR;
            } else if (priority < 7 && token == FixedToken.VERTICAL) {
                result = BIT_OR;
            } else if (priority < 6 && token == FixedToken.GREATER) {
                result = GREATER;
            } else if (priority < 6 && token == FixedToken.GREATER_EQUAL) {
                result = GREATER_EQUAL;
            } else if (priority < 6 && token == FixedToken.LESS) {
                result = LESS;
            } else if (priority < 6 && token == FixedToken.LESS_EQUAL) {
                result = LESS_EQUAL;
            } else if (priority < 6 && token == FixedToken.DOUBLE_EQUAL) {
                result = EQUAL;
            } else if (priority < 6 && token == FixedToken.EXCLAMATION_EQUAL) {
                result = NOT_EQUAL;
            } else if (priority < 5 && token == FixedToken.DOUBLE_AND) {
                result = AND;
            } else if (priority < 4 && token == FixedToken.DOUBLE_VERTICAL) {
                result = OR;
            } else if (priority <= 2 && token == FixedToken.EQUAL) {
                result = ASSIGN;
            } else if (priority <= 2 && token == FixedToken.PLUS_EQUAL) {
                result = ADD_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.MINUS_EQUAL) {
                result = SUBTRACT_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.STAR_EQUAL) {
                result = MULTIPLY_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.SLASH_EQUAL) {
                result = DIVIDE_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.PERCENT_EQUAL) {
                result = MODULO_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.AND_EQUAL) {
                result = BIT_AND_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.VERTICAL_EQUAL) {
                result = BIT_OR_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.CARET_EQUAL) {
                result = BIT_XOR_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.DOUBLE_LESS_EQUAL) {
                result = LEFT_SHIFT_ASSIGN;
            } else if (priority <= 2 && token == FixedToken.DOUBLE_GREATER_EQUAL) {
                result = RIGHT_SHIFT_ASSIGN;
            } else {
                result = null;
            }

            return result;
        }
    }
}
