using System.Collections.Immutable;
using TypeScript.Intermediate.Instruction;

namespace TypeScript.Intermediate.FunctionBody;

public class GuestBody: IFunctionBody {
    public readonly ImmutableArray<Type.IType> localTypes;
    public readonly ImmutableArray<IInstruction> instructions;

    internal GuestBody(ImmutableArray<Type.IType> localTypes, ImmutableArray<IInstruction> instructions) {
        this.localTypes = localTypes;
        this.instructions = instructions;
    }
}
