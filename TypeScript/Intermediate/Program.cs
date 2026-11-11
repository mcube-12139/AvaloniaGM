using System.Collections.Immutable;
using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate;

public class Program {
    public readonly ImmutableArray<InterfaceType> interfaces;
    public readonly ImmutableArray<ClassType> classes;
    public readonly ImmutableDictionary<(InterfaceType, ClassType), Implementation> implementations;
    public readonly ImmutableArray<Function> functions;

    internal Program(
        ImmutableArray<InterfaceType> interfaces,
        ImmutableArray<ClassType> classes,
        ImmutableDictionary<(InterfaceType, ClassType), Implementation> implementations,
        ImmutableArray<Function> functions
    ) {
        this.interfaces = interfaces;
        this.classes = classes;
        this.implementations = implementations;
        this.functions = functions;
    }
}
