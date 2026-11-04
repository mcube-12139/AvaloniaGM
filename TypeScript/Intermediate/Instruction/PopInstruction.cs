using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class PopInstruction: IInstruction {
    public readonly IType type;

    internal PopInstruction(IType type) {
        this.type = type;
    }
}
