using System;
using System.Collections.Generic;

namespace AvaloniaGM.TypeScript.Exceptions {
    public enum SemanticErrorType {
        SYMBOL_EXIST,
        SYMBOL_NOT_EXIST,
        TOO_LARGE_INTEGER,
    }

    public class SemanticException(SemanticErrorType type, string[] parameters, string source, TextPosition position) : Exception($"{source}:{position.line}:{position.column}: {formatter[type](parameters)}") {
        static readonly Dictionary<SemanticErrorType, Func<string[], string>> formatter = new() {
            { SemanticErrorType.SYMBOL_EXIST, parameters => $"符号 {parameters[0]} 已存在" },
            { SemanticErrorType.SYMBOL_NOT_EXIST, parameters => $"符号 {parameters[0]} 不存在" },
            { SemanticErrorType.TOO_LARGE_INTEGER, parameters => $"整数 {parameters[0]} 太大" },
        };
    }
}
