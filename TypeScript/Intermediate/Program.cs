using System.Collections.Immutable;
using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate;

public class Program {
    public readonly ImmutableArray<ClassType> classes;
    public readonly ImmutableArray<Function> functions;

    internal Program(ImmutableArray<ClassType> classes, ImmutableArray<Function> functions) {
        this.classes = classes;
        this.functions = functions;
    }
}
