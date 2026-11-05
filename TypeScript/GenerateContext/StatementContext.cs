using System.Collections.Immutable;
using TypeScript.Intermediate;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.GenerateContext;

internal class StatementContext: IItemContext {
    readonly List<IInstruction> instructions = [];
    readonly List<Intermediate.Type.IType> localTypes = [];
    readonly List<Function> functions = [];

    internal ImmutableArray<IInstruction> GetInstructions() => [.. instructions];
    internal ImmutableArray<Intermediate.Type.IType> GetLocalTypes() => [.. localTypes];
    internal IEnumerable<Function> GetFunctions() => functions;

    void IItemContext.AddFunction(Function fun) {
        functions.Add(fun);
    }

    internal void AddInstruction(IInstruction instruction) {
        instructions.Add(instruction);
    }

    internal void AddInstructionWithPosition(IInstruction instruction, SourcePosition position) {
        instructions.Add(instruction);
    }

    internal int GetLocalCount() => localTypes.Count;

    internal int GetInstructionCount() => instructions.Count;

    internal void AddLocalType(Intermediate.Type.IType type) {
        localTypes.Add(type);
    }
}
