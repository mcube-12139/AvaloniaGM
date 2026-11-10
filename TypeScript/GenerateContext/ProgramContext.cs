using TypeScript.Intermediate;
using TypeScript.Intermediate.Type;

namespace TypeScript.GenerateContext;

internal class ProgramContext: IItemContext {
    readonly List<ClassType> classes = [];
    readonly List<Function> functions = [];

    ProgramContext IItemContext.GetProgramContext() => this;
    void IItemContext.AddClass(ClassType c) => classes.Add(c);
    void IItemContext.AddFunction(Function fun) => functions.Add(fun);

    internal IEnumerable<ClassType> GetClasses() => classes;
    internal IEnumerable<Function> GetFunctions() => functions;
}
