using TypeScript.Intermediate;
using TypeScript.Intermediate.Type;

namespace TypeScript.GenerateContext;

internal interface IItemContext {
    ProgramContext GetProgramContext();
    void AddInterface(InterfaceType i);
    void AddClass(ClassType c);
    void AddImpl((InterfaceType, ClassType) typePair, Implementation impl);
    void AddFunction(Function fun);
}
