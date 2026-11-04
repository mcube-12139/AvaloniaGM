using System.Collections.Immutable;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Intermediate.FunctionBody;

public class GuestBody: IFunctionBody {
    public readonly ImmutableArray<IInstruction> instructions;

    internal GuestBody(ImmutableArray<IInstruction> instructions) {
        this.instructions = instructions;
    }
}
