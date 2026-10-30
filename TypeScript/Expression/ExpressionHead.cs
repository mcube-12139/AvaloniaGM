using TypeScript.Type;

namespace TypeScript.Expression;

internal class ExpressionHead(SourcePosition position) {
    internal readonly SourcePosition position = position;
    internal IType? targetType;
}

