using TypeScript.Intermediate;

namespace TypeScript.GenerateContext;

internal class ProgramContext: IItemContext {
    readonly List<Function> functions = [];

    internal IEnumerable<Function> GetFunctions() => functions;

    void IItemContext.AddFunction(Function fun) {
        functions.Add(fun);
    }
}
