using AvaloniaGM.TypeScript.Exceptions;
using AvaloniaGM.TypeScript.Expressions;
using System;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Types {
    internal class PrimitiveType(
        string name,
        Action<IExpression, TextPosition, Generator> multiply,
        Action<IExpression, TextPosition, Generator> divide,
        Action<IExpression, TextPosition, Generator> modulo,
        Action<IExpression, TextPosition, Generator> add,
        Action<IExpression, TextPosition, Generator> subtract,
        Action<IExpression, TextPosition, Generator> leftShift,
        Action<IExpression, TextPosition, Generator> rightShift,
        Action<IExpression, TextPosition, Generator> bitAnd,
        Action<IExpression, TextPosition, Generator> bitXor,
        Action<IExpression, TextPosition, Generator> bitOr,
        Action<IExpression, TextPosition, Generator> greater,
        Action<IExpression, TextPosition, Generator> greaterEqual,
        Action<IExpression, TextPosition, Generator> less,
        Action<IExpression, TextPosition, Generator> lessEqual,
        Action<IExpression, TextPosition, Generator> equal,
        Action<IExpression, TextPosition, Generator> notEqual,
        Action<IExpression, TextPosition, Generator> assign,
        Action<IExpression, TextPosition, Generator> addAssign,
        Action<IExpression, TextPosition, Generator> subtractAssign,
        Action<IExpression, TextPosition, Generator> multiplyAssign,
        Action<IExpression, TextPosition, Generator> divideAssign,
        Action<IExpression, TextPosition, Generator> moduloAssign,
        Action<IExpression, TextPosition, Generator> bitAndAssign,
        Action<IExpression, TextPosition, Generator> bitOrAssign,
        Action<IExpression, TextPosition, Generator> bitXorAssign,
        Action<IExpression, TextPosition, Generator> leftShiftAssign,
        Action<IExpression, TextPosition, Generator> rightShiftAssign,
        Func<IType, IType, TextPosition, Generator, IType> getMultiplyResultType,
        Func<IType, IType, TextPosition, Generator, IType> getDivideResultType,
        Func<IType, IType, TextPosition, Generator, IType> getModuloResultType,
        Func<IType, IType, TextPosition, Generator, IType> getAddResultType,
        Func<IType, IType, TextPosition, Generator, IType> getSubtractResultType,
        Func<IType, IType, TextPosition, Generator, IType> getLeftShiftResultType,
        Func<IType, IType, TextPosition, Generator, IType> getRightShiftResultType,
        Func<IType, IType, TextPosition, Generator, IType> getBitAndResultType,
        Func<IType, IType, TextPosition, Generator, IType> getBitXorResultType,
        Func<IType, IType, TextPosition, Generator, IType> getBitOrResultType,
        Func<IType, IType, TextPosition, Generator, IType> getGreaterResultType,
        Func<IType, IType, TextPosition, Generator, IType> getGreaterEqualResultType,
        Func<IType, IType, TextPosition, Generator, IType> getLessResultType,
        Func<IType, IType, TextPosition, Generator, IType> getLessEqualResultType,
        Func<IType, IType, TextPosition, Generator, IType> getEqualResultType,
        Func<IType, IType, TextPosition, Generator, IType> getNotEqualResultType
    ) : IType {
        internal static PrimitiveType INTEGER = new("int", (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int * {rightType.GetAppearance()}"], position);
            }
            generator.Multiply();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int / {rightType.GetAppearance()}"], position);
            }
            generator.Divide();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int % {rightType.GetAppearance()}"], position);
            }
            generator.Modulo();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int - {rightType.GetAppearance()}"], position);
            }
            generator.Subtract();
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int << ?"], position);
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >> ?"], position);
        }, (right, position, generator) => {
            // BitAnd
            generator.Convert(UndertaleInstruction.DataType.Int32);
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int & {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Int32);
            generator.BitAnd();
        }, (right, position, generator) => {
            // BitXor
            generator.Convert(UndertaleInstruction.DataType.Int32);
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int ^ {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Int32);
            generator.BitXor();
        }, (right, position, generator) => {
            // BitOr
            generator.Convert(UndertaleInstruction.DataType.Int32);
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int | {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Int32);
            generator.BitOr();
        }, (right, position, generator) => {
            // Greater
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int > {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.GT);
        }, (right, position, generator) => {
            // GreaterEqual
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >= {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.GTE);
        }, (right, position, generator) => {
            // Less
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int < {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.LT);
        }, (right, position, generator) => {
            // LessEqual
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int <= {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.LTE);
        }, (right, position, generator) => {
            // Equal
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int == {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.EQ);
        }, (right, position, generator) => {
            // NotEqual
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int != {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.NEQ);
        }, (right, position, generator) => {
            // Assign
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int = {rightType.GetAppearance()}"], position);
            }
        }, (right, position, generator) => {
            // AddAssign
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int += {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            // SubtractAssign
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int -= {rightType.GetAppearance()}"], position);
            }
            generator.Subtract();
        }, (right, position, generator) => {
            // MultiplyAssign
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int *= {rightType.GetAppearance()}"], position);
            }
            generator.Multiply();
        }, (right, position, generator) => {
            // DivideAssign
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int /= {rightType.GetAppearance()}"], position);
            }
            generator.Divide();
        }, (right, position, generator) => {
            // ModuloAssign
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int %= {rightType.GetAppearance()}"], position);
            }
            generator.Modulo();
        }, (right, position, generator) => {
            // BitAndAssign
            generator.Convert(UndertaleInstruction.DataType.Int32);
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int &= {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Int32);
            generator.BitAnd();
        }, (right, position, generator) => {
            // BitOrAssign
            generator.Convert(UndertaleInstruction.DataType.Int32);
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int |= {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Int32);
            generator.BitOr();
        }, (right, position, generator) => {
            // BitXorAssign
            generator.Convert(UndertaleInstruction.DataType.Int32);
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int ^= {rightType.GetAppearance()}"], position);
            }
            generator.Convert(UndertaleInstruction.DataType.Int32);
            generator.BitXor();
        }, (right, position, generator) => {
            // LeftShiftAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int <<= ?"], position);
        }, (right, position, generator) => {
            // RightShiftAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >>= ?"], position);
        }, (self, rightType, position, generator) => {
            // getMultiplyResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getDivideResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getModuloResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getAddResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getSubtractResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLeftShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getRightShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitAndResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitXorResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitOrResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getNotEqualResultType
            return self;
        });
        internal static PrimitiveType DOUBLE = new("double", (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int * {rightType.GetAppearance()}"], position);
            }
            generator.Multiply();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int / {rightType.GetAppearance()}"], position);
            }
            generator.Divide();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int % {rightType.GetAppearance()}"], position);
            }
            generator.Modulo();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int - {rightType.GetAppearance()}"], position);
            }
            generator.Subtract();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int << {rightType.GetAppearance()}"], position);
            }
            generator.LeftShift();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >> {rightType.GetAppearance()}"], position);
            }
            generator.RightShift();
        }, (right, position, generator) => {
            // BitAnd
        }, (right, position, generator) => {
            // BitXor
        }, (right, position, generator) => {
            // BitOr
        }, (right, position, generator) => {
            // Greater
        }, (right, position, generator) => {
            // GreaterEqual
        }, (right, position, generator) => {
            // Less
        }, (right, position, generator) => {
            // LessEqual
        }, (right, position, generator) => {
            // Equal
        }, (right, position, generator) => {
            // NotEqual
        }, (right, position, generator) => {
            // Assign
        }, (right, position, generator) => {
            // AddAssign
        }, (right, position, generator) => {
            // SubtractAssign
        }, (right, position, generator) => {
            // MultiplyAssign
        }, (right, position, generator) => {
            // DivideAssign
        }, (right, position, generator) => {
            // ModuloAssign
        }, (right, position, generator) => {
            // BitAndAssign
        }, (right, position, generator) => {
            // BitOrAssign
        }, (right, position, generator) => {
            // BitXorAssign
        }, (right, position, generator) => {
            // LeftShiftAssign
        }, (right, position, generator) => {
            // RightShiftAssign
        }, (self, rightType, position, generator) => {
            // getMultiplyResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getDivideResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getModuloResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getAddResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getSubtractResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLeftShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getRightShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitAndResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitXorResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitOrResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getNotEqualResultType
            return self;
        });
        internal static PrimitiveType LONG = new("long", (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int * {rightType.GetAppearance()}"], position);
            }
            generator.Multiply();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int / {rightType.GetAppearance()}"], position);
            }
            generator.Divide();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int % {rightType.GetAppearance()}"], position);
            }
            generator.Modulo();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int - {rightType.GetAppearance()}"], position);
            }
            generator.Subtract();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int << {rightType.GetAppearance()}"], position);
            }
            generator.LeftShift();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >> {rightType.GetAppearance()}"], position);
            }
            generator.RightShift();
        }, (right, position, generator) => {
            // BitAnd
        }, (right, position, generator) => {
            // BitXor
        }, (right, position, generator) => {
            // BitOr
        }, (right, position, generator) => {
            // Greater
        }, (right, position, generator) => {
            // GreaterEqual
        }, (right, position, generator) => {
            // Less
        }, (right, position, generator) => {
            // LessEqual
        }, (right, position, generator) => {
            // Equal
        }, (right, position, generator) => {
            // NotEqual
        }, (right, position, generator) => {
            // Assign
        }, (right, position, generator) => {
            // AddAssign
        }, (right, position, generator) => {
            // SubtractAssign
        }, (right, position, generator) => {
            // MultiplyAssign
        }, (right, position, generator) => {
            // DivideAssign
        }, (right, position, generator) => {
            // ModuloAssign
        }, (right, position, generator) => {
            // BitAndAssign
        }, (right, position, generator) => {
            // BitOrAssign
        }, (right, position, generator) => {
            // BitXorAssign
        }, (right, position, generator) => {
            // LeftShiftAssign
        }, (right, position, generator) => {
            // RightShiftAssign
        }, (self, rightType, position, generator) => {
            // getMultiplyResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getDivideResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getModuloResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getAddResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getSubtractResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLeftShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getRightShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitAndResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitXorResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitOrResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getNotEqualResultType
            return self;
        });
        internal static PrimitiveType STRING = new("string", (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string * ?"], position);
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string / ?"], position);
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string % ?"], position);
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string + {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string - ?"], position);
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string << ?"], position);
        }, (right, position, generator) => {
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string >> ?"], position);
        }, (right, position, generator) => {
            // BitAnd
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string & ?"], position);
        }, (right, position, generator) => {
            // BitXor
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string ^ ?"], position);
        }, (right, position, generator) => {
            // BitOr
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string | ?"], position);
        }, (right, position, generator) => {
            // Greater
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string > {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.GT);
        }, (right, position, generator) => {
            // GreaterEqual
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string >= {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.GTE);
        }, (right, position, generator) => {
            // Less
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string < {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.LT);
        }, (right, position, generator) => {
            // LessEqual
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string <= {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.LTE);
        }, (right, position, generator) => {
            // Equal
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string == {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.EQ);
        }, (right, position, generator) => {
            // NotEqual
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string != {rightType.GetAppearance()}"], position);
            }
            generator.Compare(UndertaleInstruction.ComparisonType.NEQ);
        }, (right, position, generator) => {
            // Assign
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string = {rightType.GetAppearance()}"], position);
            }
        }, (right, position, generator) => {
            // AddAssign
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != STRING) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string += {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            // SubtractAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string -= ?"], position);
        }, (right, position, generator) => {
            // MultiplyAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string *= ?"], position);
        }, (right, position, generator) => {
            // DivideAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string /= ?"], position);
        }, (right, position, generator) => {
            // ModuloAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string %= ?"], position);
        }, (right, position, generator) => {
            // BitAndAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string &= ?"], position);
        }, (right, position, generator) => {
            // BitOrAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string |= ?"], position);
        }, (right, position, generator) => {
            // BitXorAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string ^= ?"], position);
        }, (right, position, generator) => {
            // LeftShiftAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string <<= ?"], position);
        }, (right, position, generator) => {
            // RightShiftAssign
            throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"string >>= ?"], position);
        }, (self, rightType, position, generator) => {
            // getMultiplyResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getDivideResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getModuloResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getAddResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getSubtractResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLeftShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getRightShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitAndResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitXorResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitOrResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getNotEqualResultType
            return self;
        });
        internal static PrimitiveType BOOLEAN = new("boolean", (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int * {rightType.GetAppearance()}"], position);
            }
            generator.Multiply();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int / {rightType.GetAppearance()}"], position);
            }
            generator.Divide();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int % {rightType.GetAppearance()}"], position);
            }
            generator.Modulo();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int + {rightType.GetAppearance()}"], position);
            }
            generator.Add();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int - {rightType.GetAppearance()}"], position);
            }
            generator.Subtract();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int << {rightType.GetAppearance()}"], position);
            }
            generator.LeftShift();
        }, (right, position, generator) => {
            right.Evaluate(generator);
            IType rightType = right.GetResultType(generator);
            if (rightType != INTEGER) {
                throw generator.SemanticError(SemanticErrorType.OPERATION_NOT_EXIST, [$"int >> {rightType.GetAppearance()}"], position);
            }
            generator.RightShift();
        }, (right, position, generator) => {
            // BitAnd
        }, (right, position, generator) => {
            // BitXor
        }, (right, position, generator) => {
            // BitOr
        }, (right, position, generator) => {
            // Greater
        }, (right, position, generator) => {
            // GreaterEqual
        }, (right, position, generator) => {
            // Less
        }, (right, position, generator) => {
            // LessEqual
        }, (right, position, generator) => {
            // Equal
        }, (right, position, generator) => {
            // NotEqual
        }, (right, position, generator) => {
            // Assign
        }, (right, position, generator) => {
            // AddAssign
        }, (right, position, generator) => {
            // SubtractAssign
        }, (right, position, generator) => {
            // MultiplyAssign
        }, (right, position, generator) => {
            // DivideAssign
        }, (right, position, generator) => {
            // ModuloAssign
        }, (right, position, generator) => {
            // BitAndAssign
        }, (right, position, generator) => {
            // BitOrAssign
        }, (right, position, generator) => {
            // BitXorAssign
        }, (right, position, generator) => {
            // LeftShiftAssign
        }, (right, position, generator) => {
            // RightShiftAssign
        }, (self, rightType, position, generator) => {
            // getMultiplyResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getDivideResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getModuloResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getAddResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getSubtractResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLeftShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getRightShiftResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitAndResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitXorResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getBitOrResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getGreaterEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getLessEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getEqualResultType
            return self;
        }, (self, rightType, position, generator) => {
            // getNotEqualResultType
            return self;
        });

        public string GetAppearance() {
            return name;
        }

        bool IType.IsType(IType other) {
            return other == this;
        }

        void IType.Multiply(IExpression right, TextPosition position, Generator generator) {
            multiply(right, position, generator);
        }

        void IType.Divide(IExpression right, TextPosition position, Generator generator) {
            divide(right, position, generator);
        }

        void IType.Modulo(IExpression right, TextPosition position, Generator generator) {
            modulo(right, position, generator);
        }

        void IType.Add(IExpression right, TextPosition position, Generator generator) {
            add(right, position, generator);
        }

        void IType.Subtract(IExpression right, TextPosition position, Generator generator) {
            subtract(right, position, generator);
        }

        void IType.LeftShift(IExpression right, TextPosition position, Generator generator) {
            leftShift(right, position, generator);
        }

        void IType.RightShift(IExpression right, TextPosition position, Generator generator) {
            rightShift(right, position, generator);
        }

        void IType.BitAnd(IExpression right, TextPosition position, Generator generator) {
            bitAnd(right, position, generator);
        }

        void IType.BitXor(IExpression right, TextPosition position, Generator generator) {
            bitXor(right, position, generator);
        }

        void IType.BitOr(IExpression right, TextPosition position, Generator generator) {
            bitOr(right, position, generator);
        }

        void IType.Greater(IExpression right, TextPosition position, Generator generator) {
            greater(right, position, generator);
        }

        void IType.GreaterEqual(IExpression right, TextPosition position, Generator generator) {
            greaterEqual(right, position, generator);
        }

        void IType.Less(IExpression right, TextPosition position, Generator generator) {
            less(right, position, generator);
        }

        void IType.LessEqual(IExpression right, TextPosition position, Generator generator) {
            lessEqual(right, position, generator);
        }

        void IType.Equal(IExpression right, TextPosition position, Generator generator) {
            equal(right, position, generator);
        }

        void IType.NotEqual(IExpression right, TextPosition position, Generator generator) {
            notEqual(right, position, generator);
        }

        void IType.Assign(IExpression right, TextPosition position, Generator generator) {
            assign(right, position, generator);
        }

        void IType.AddAssign(IExpression right, TextPosition position, Generator generator) {
            addAssign(right, position, generator);
        }

        void IType.SubtractAssign(IExpression right, TextPosition position, Generator generator) {
            subtractAssign(right, position, generator);
        }

        void IType.MultiplyAssign(IExpression right, TextPosition position, Generator generator) {
            multiplyAssign(right, position, generator);
        }

        void IType.DivideAssign(IExpression right, TextPosition position, Generator generator) {
            divideAssign(right, position, generator);
        }

        void IType.ModuloAssign(IExpression right, TextPosition position, Generator generator) {
            moduloAssign(right, position, generator);
        }

        void IType.BitAndAssign(IExpression right, TextPosition position, Generator generator) {
            bitAndAssign(right, position, generator);
        }

        void IType.BitOrAssign(IExpression right, TextPosition position, Generator generator) {
            bitOrAssign(right, position, generator);
        }

        void IType.BitXorAssign(IExpression right, TextPosition position, Generator generator) {
            bitXorAssign(right, position, generator);
        }

        void IType.LeftShiftAssign(IExpression right, TextPosition position, Generator generator) {
            leftShiftAssign(right, position, generator);
        }

        void IType.RightShiftAssign(IExpression right, TextPosition position, Generator generator) {
            rightShiftAssign(right, position, generator);
        }

        IType IType.GetMultiplyResultType(IType rightType, TextPosition position, Generator generator) {
            return getMultiplyResultType(this, rightType, position, generator);
        }

        IType IType.GetDivideResultType(IType rightType, TextPosition position, Generator generator) {
            return getDivideResultType(this, rightType, position, generator);
        }

        IType IType.GetModuloResultType(IType rightType, TextPosition position, Generator generator) {
            return getModuloResultType(this, rightType, position, generator);
        }

        IType IType.GetAddResultType(IType rightType, TextPosition position, Generator generator) {
            return getAddResultType(this, rightType, position, generator);
        }

        IType IType.GetSubtractResultType(IType rightType, TextPosition position, Generator generator) {
            return getSubtractResultType(this, rightType, position, generator);
        }

        IType IType.GetLeftShiftResultType(IType rightType, TextPosition position, Generator generator) {
            return getLeftShiftResultType(this, rightType, position, generator);
        }

        IType IType.GetRightShiftResultType(IType rightType, TextPosition position, Generator generator) {
            return getRightShiftResultType(this, rightType, position, generator);
        }

        IType IType.GetBitAndResultType(IType rightType, TextPosition position, Generator generator) {
            return getBitAndResultType(this, rightType, position, generator);
        }

        IType IType.GetBitXorResultType(IType rightType, TextPosition position, Generator generator) {
            return getBitXorResultType(this, rightType, position, generator);
        }

        IType IType.GetBitOrResultType(IType rightType, TextPosition position, Generator generator) {
            return getBitOrResultType(this, rightType, position, generator);
        }

        IType IType.GetGreaterResultType(IType rightType, TextPosition position, Generator generator) {
            return getGreaterResultType(this, rightType, position, generator);
        }

        IType IType.GetGreaterEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return getGreaterEqualResultType(this, rightType, position, generator);
        }

        IType IType.GetLessResultType(IType rightType, TextPosition position, Generator generator) {
            return getLessResultType(this, rightType, position, generator);
        }

        IType IType.GetLessEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return getLessEqualResultType(this, rightType, position, generator);
        }

        IType IType.GetEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return getEqualResultType(this, rightType, position, generator);
        }

        IType IType.GetNotEqualResultType(IType rightType, TextPosition position, Generator generator) {
            return getNotEqualResultType(this, rightType, position, generator);
        }

        IType IType.GetCallResultType(TextPosition position, Generator generator) {
            throw generator.SemanticError(SemanticErrorType.NOT_CALLABLE, [GetAppearance()], position);
        }

        IType IType.GetIndexResultType(IType indexType, TextPosition position, Generator generator) {
            throw new NotImplementedException();
        }

        IType IType.GetSetIndexResultType(IType rightType, IType indexType, TextPosition position, Generator generator) {
            throw new NotImplementedException();
        }

        void IType.GetIndex(UndertaleVariable variable, IExpression index, TextPosition position, Generator generator) {
            throw new NotImplementedException();
        }

        void IType.SetIndex(UndertaleVariable variable, IExpression right, IExpression index, TextPosition position, Generator generator) {
            throw new NotImplementedException();
        }
    }
}
