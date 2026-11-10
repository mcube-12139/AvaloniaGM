using TypeScript.Intermediate;
using TypeScript.Intermediate.Type;

namespace TypeScript.GenerateContext;

internal interface IItemContext {
    ProgramContext GetProgramContext();
    void AddClass(ClassType c);
    void AddFunction(Function fun);
}
