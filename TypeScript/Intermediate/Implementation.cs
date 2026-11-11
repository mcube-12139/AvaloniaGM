using System.Collections.Immutable;

namespace TypeScript.Intermediate;

public class Implementation {
    public readonly ImmutableDictionary<VirtualFunction, Function> functions;

    internal Implementation(ImmutableDictionary<VirtualFunction, Function> functions) {
        this.functions = functions;
    }
}
