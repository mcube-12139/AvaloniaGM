using System.Collections.Immutable;
using TypeScript.Intermediate;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.GenerateContext;

internal class StatementContext: IItemContext {
    readonly List<IInstruction> instructions = [];
    readonly List<Function> functions = [];

    internal ImmutableArray<IInstruction> GetInstructions() => [.. instructions];

    void IItemContext.AddFunction(Function fun) {
        functions.Add(fun);
    }

    internal void AddInstruction(IInstruction instruction, SourcePosition position) {
        instructions.Add(instruction);
    }

    internal IEnumerable<Function> GetFunctions() => functions;
}
