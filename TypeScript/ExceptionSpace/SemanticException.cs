using System;
using System.Collections.Generic;

namespace TypeScript.ExceptionSpace;

public enum SemanticErrorType {
    SYMBOL_EXIST,
    SYMBOL_NOT_EXIST,
    LABEL_NOT_EXIST,
    TOO_LARGE_INTEGER,
    NOT_CALLABLE,
    NOT_WRITEABLE,
    NOT_VALUE,
    NOT_TYPE,
    NOT_PLACE,
    MAY_NOT_RETURN,
    MAY_NOT_INITIALIZED,
    OPERATION_NOT_EXIST,
    NOT_IN_LOOP,
    WRONG_PARAMETER_COUNT,
    WRONG_TYPE,
    UNKNOWN_TYPE,
}

public class SemanticException(SemanticErrorType type, string[] parameters, string source, TextPosition position) : Exception($"{source}:{position.line}:{position.column}: {formatter[type](parameters)}") {
    static readonly Dictionary<SemanticErrorType, Func<string[], string>> formatter = new() {
        { SemanticErrorType.SYMBOL_EXIST, parameters => $"符号 {parameters[0]} 已存在" },
        { SemanticErrorType.SYMBOL_NOT_EXIST, parameters => $"符号 {parameters[0]} 不存在" },
        { SemanticErrorType.LABEL_NOT_EXIST, parameters => $"标签 {parameters[0]} 不存在" },
        { SemanticErrorType.TOO_LARGE_INTEGER, parameters => $"整数 {parameters[0]} 太大" },
        { SemanticErrorType.NOT_CALLABLE, parameters => $"{parameters[0]} 不是可调用的" },
        { SemanticErrorType.NOT_WRITEABLE, parameters => $"{parameters[0]} 不是可写的" },
        { SemanticErrorType.NOT_VALUE, parameters => $"{parameters[0]} 不是值" },
        { SemanticErrorType.NOT_TYPE, parameters => $"{parameters[0]} 不是类型" },
        { SemanticErrorType.NOT_PLACE, parameters => "不是位置式" },
        { SemanticErrorType.MAY_NOT_RETURN, parameters => $"{parameters[0]} 可能不返回值" },
        { SemanticErrorType.MAY_NOT_INITIALIZED, parameters => $"{parameters[0]} 可能未初始化" },
        { SemanticErrorType.OPERATION_NOT_EXIST, parameters => $"{parameters[0]} 运算不存在" },
        { SemanticErrorType.NOT_IN_LOOP, parameters => "不在循环中" },
        { SemanticErrorType.WRONG_PARAMETER_COUNT, parameters => $"参数数量 {parameters[0]} 不是所需的 {parameters[1]}" },
        { SemanticErrorType.WRONG_TYPE, parameters => $"类型 {parameters[0]} 不是所需的 {parameters[1]}" },
        { SemanticErrorType.UNKNOWN_TYPE, parameters => $"未知类型" },
    };
}
