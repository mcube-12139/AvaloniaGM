using System.Collections.Immutable;

namespace TypeScript.Intermediate.Type;

public class InterfaceType: IType {
    public readonly string name;
    public readonly ImmutableArray<VirtualFunction> functions;

    internal InterfaceType(
        string name,
        ImmutableArray<VirtualFunction> functions
    ) {
        this.name = name;
        this.functions = functions;
    }
}
