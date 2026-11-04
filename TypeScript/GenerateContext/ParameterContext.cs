using System.Collections.Immutable;

namespace TypeScript.GenerateContext;

internal class ParameterContext {
    readonly List<Intermediate.Parameter> parameters = [];

    internal ImmutableArray<Intermediate.Parameter> GetParameters() => [.. parameters];

    internal void AddParameter(Intermediate.Parameter fun) {
        parameters.Add(fun);
    }
}
