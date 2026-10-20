using TypeScript.Type;

namespace TypeScript.Parameter;

public class ThisParameter(SourcePosition position) {
    public readonly SourcePosition position = position;
}
