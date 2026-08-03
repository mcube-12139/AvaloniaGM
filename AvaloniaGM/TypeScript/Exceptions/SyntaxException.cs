using System;
using System.Collections.Generic;

namespace AvaloniaGM.TypeScript.Exceptions {
    public enum SyntaxErrorType {
        CANNOT_BE_HERE,
        CANNOT_END_HERE
    }

    public class SyntaxException(SyntaxErrorType type, string[] parameters, string source, TextPosition position) : Exception($"{source}:{position.line}:{position.column}: {formatter[type](parameters)}") {
        internal static Dictionary<SyntaxErrorType, Func<string[], string>> formatter = new() {
            { SyntaxErrorType.CANNOT_BE_HERE, parameters => $"{parameters[0]} 不能在这里" },
            { SyntaxErrorType.CANNOT_END_HERE, parameters => "不能在这里结束" }
        };
    }
}
