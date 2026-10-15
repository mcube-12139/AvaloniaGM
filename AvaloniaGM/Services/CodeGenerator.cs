using AvaloniaEdit.Editing;
using AvaloniaEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TypeScript;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.Type;
using UndertaleModLib;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleInstruction;

namespace AvaloniaGM.Services;

internal class CodeGenerator {
    Dictionary<string, UndertaleCode> funCodes = [];
    UndertaleData? data;
    UndertaleCode? nowCode;
    UndertaleCodeLocals? codeLocals;
    uint byteCount;
    // 这个 1 是 arguments
    uint nextLocalIndex = 1;
    Stack<DataType> types = [];
    List<UndertaleInstruction> instructions = [];

    void PushType(DataType type) {
        types.Push(type);
    }

    void PopType() {
        types.Pop();
    }

    void Convert(DataType target) {
        DataType source = types.Pop();

        if (source != target) {
            instructions.Add(new() {
                Kind = Opcode.Conv,
                Type1 = source,
                Type2 = target
            });
            byteCount += 4;
        }

        PushType(target);
    }

    void PopUnused() {
        DataType type = types.Pop();
        instructions.Add(new() {
            Kind = Opcode.Popz,
            Type1 = type
        });
        byteCount += 4;
    }

    void Call(UndertaleFunction fun, int parameterCount) {
        for (int i = parameterCount; i != 0; --i) {
            PopType();
        }

        instructions.Add(new() {
            Kind = Opcode.Call,
            Type1 = DataType.Int32,
            ArgumentsCount = (ushort)parameterCount,
            ValueFunction = fun
        });
        byteCount += 8;

        PushType(DataType.Variable);
    }

    void CallSymbol(IValueSymbol valueSymbol) {
        switch (valueSymbol) {
            case FunctionSymbol functionSymbol: {
                if (functionSymbol.hostName != null) {
                    UndertaleFunction fun = data!.Functions.EnsureDefined(functionSymbol.hostName, data.Strings);
                    Call(fun, functionSymbol.GetSharkType().parameterTypes.Length);
                } else {
                    throw new NotImplementedException();
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ExpressionCall(IExpression called) {
        switch (called) {
            case PathExpression pathExpression: {
                CallSymbol(pathExpression.GetSymbol());
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void EvaluateExpression(IExpression expression) {
        switch (expression) {
            case IntegerExpression integerExpression: {
                int value = int.Parse(integerExpression.valueStr);

                instructions.Add(new() {
                    Kind = Opcode.Push,
                    Type1 = DataType.Int32,
                    Type2 = DataType.Double,
                    ValueInt = value
                });
                byteCount += 8;

                PushType(DataType.Int32);
                break;
            }
            case PathExpression pathExpression: {
                throw new NotImplementedException();
            }
            case CallExpression callExpression: {
                for (int i = callExpression.parameters.Length - 1; i != -1; --i) {
                    EvaluateExpression(callExpression.parameters[i]);
                    Convert(DataType.Variable);
                }
                ExpressionCall(callExpression.called);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    void ExecuteStatement(IStatement statement) {
        switch (statement) {
            case ExpressionStatement expressionStatement:
                EvaluateExpression(expressionStatement.expression);
                if (expressionStatement.expression.GetResultType() != TupleType.EMPTY) {
                    PopUnused();
                }
                break;
            case BlockStatement blockStatement:
                ExecuteBlockStatement(blockStatement);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void ExecuteBlockStatement(BlockStatement blockStatement) {
        foreach (IStatement statement in blockStatement.statements) {
            ExecuteStatement(statement);
        }
    }

    void GenerateItem(IItem item) {
        switch (item) {
            case FunctionItem funItem: {
                UndertaleCode? code = funCodes.GetValueOrDefault(funItem.name);
                if (code != null) {
                    nowCode = code;
                    codeLocals = data!.CodeLocals.For(code);
                } else {
                    throw new NotImplementedException();
                }
                ExecuteBlockStatement(funItem.body);

                code.Replace(instructions);
                code.Length = byteCount;
                code.Offset = 0;
                code.ArgumentsCount = 0;
                code.LocalsCount = nextLocalIndex;
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    internal void Generate(UndertaleData data, FileNode[] nodes, Dictionary<string, UndertaleCode> funCodes) {
        this.data = data;
        this.funCodes.AddRange(funCodes);

        foreach (FileNode node in nodes) {
            foreach (IItem item in node.items) {
                GenerateItem(item);
            }
        }

        this.funCodes.Clear();
        this.data = null;
    }
}
