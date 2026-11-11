using System.Collections.Immutable;
using TypeScript.Intermediate;
using TypeScript.Intermediate.Instruction;
using TypeScript.Intermediate.Type;

namespace TypeScript.GenerateContext;

internal class StatementContext: IItemContext {
    readonly ProgramContext programContext;
    readonly List<IInstruction> instructions = [];
    readonly List<IType> localTypes = [];

    ProgramContext IItemContext.GetProgramContext() => programContext;

    internal ImmutableArray<IInstruction> GetInstructions() => [.. instructions];
    internal ImmutableArray<IType> GetLocalTypes() => [.. localTypes];

    internal StatementContext(ProgramContext programContext) {
        this.programContext = programContext;
    }

    void IItemContext.AddInterface(InterfaceType i) => ((IItemContext)programContext).AddInterface(i);
    void IItemContext.AddClass(ClassType c) => ((IItemContext)programContext).AddClass(c);
    void IItemContext.AddImpl((InterfaceType, ClassType) typePair, Implementation impl) => ((IItemContext)programContext).AddImpl(typePair, impl);
    void IItemContext.AddFunction(Function fun) => ((IItemContext)programContext).AddFunction(fun);

    internal void AddInstruction(IInstruction instruction) {
        instructions.Add(instruction);
    }

    internal void AddInstructionWithPosition(IInstruction instruction, SourcePosition _1) {
        instructions.Add(instruction);
    }

    internal int GetLocalCount() => localTypes.Count;

    internal int GetInstructionCount() => instructions.Count;

    internal void AddLocalType(Intermediate.Type.IType type) {
        localTypes.Add(type);
    }
}
