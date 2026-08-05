using System;
using System.Collections.Generic;

namespace AvaloniaGM.TypeScript.Exceptions {
    public enum SemanticErrorType {
        SYMBOL_EXIST,
        SYMBOL_NOT_EXIST,
        LABEL_NOT_EXIST,
        TOO_LARGE_INTEGER,
        NOT_CALLABLE,
        NOT_WRITEABLE,
        NOT_PLACE,
        OPERATION_NOT_EXIST,
        NOT_IN_LOOP,
    }

    public class SemanticException(SemanticErrorType type, string[] parameters, string source, TextPosition position) : Exception($"{source}:{position.line}:{position.column}: {formatter[type](parameters)}") {
        static readonly Dictionary<SemanticErrorType, Func<string[], string>> formatter = new() {
            { SemanticErrorType.SYMBOL_EXIST, parameters => $"符号 {parameters[0]} 已存在" },
            { SemanticErrorType.SYMBOL_NOT_EXIST, parameters => $"符号 {parameters[0]} 不存在" },
            { SemanticErrorType.LABEL_NOT_EXIST, parameters => $"标签 {parameters[0]} 不存在" },
            { SemanticErrorType.TOO_LARGE_INTEGER, parameters => $"整数 {parameters[0]} 太大" },
            { SemanticErrorType.NOT_CALLABLE, parameters => $"{parameters[0]} 不是可调用的" },
            { SemanticErrorType.NOT_WRITEABLE, parameters => $"{parameters[0]} 不是可写的" },
            { SemanticErrorType.NOT_PLACE, parameters => "不是位置式" },
            { SemanticErrorType.OPERATION_NOT_EXIST, parameters => $"{parameters[0]} 运算不存在" },
            { SemanticErrorType.NOT_IN_LOOP, parameters => "不在循环中" },
        };
    }
}
