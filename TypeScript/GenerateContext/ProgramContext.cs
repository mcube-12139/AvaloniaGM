using System.Collections.Immutable;
using TypeScript.Intermediate;
using TypeScript.Intermediate.Type;

namespace TypeScript.GenerateContext;

internal class ProgramContext: IItemContext {
    readonly List<InterfaceType> interfaces = [];
    readonly List<ClassType> classes = [];
    readonly Dictionary<(InterfaceType, ClassType), Implementation> impls = [];
    readonly List<Function> functions = [];

    ProgramContext IItemContext.GetProgramContext() => this;
    void IItemContext.AddInterface(InterfaceType c) => interfaces.Add(c);
    void IItemContext.AddClass(ClassType c) => classes.Add(c);
    void IItemContext.AddImpl((InterfaceType, ClassType) typePair, Implementation impl) => impls.Add(typePair, impl);
    void IItemContext.AddFunction(Function fun) => functions.Add(fun);

    internal IEnumerable<InterfaceType> GetInterfaces() => interfaces;
    internal IEnumerable<ClassType> GetClasses() => classes;
    internal ImmutableDictionary<(InterfaceType, ClassType), Implementation> GetImpls() => ImmutableDictionary.CreateRange(impls);
    internal IEnumerable<Function> GetFunctions() => functions;
}
