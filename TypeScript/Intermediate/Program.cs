using System.Collections.Immutable;

namespace TypeScript.Intermediate;

public class Program {
    public readonly ImmutableArray<Function> functions;

    internal Program(ImmutableArray<Function> functions) {
        this.functions = functions;
    }
}
