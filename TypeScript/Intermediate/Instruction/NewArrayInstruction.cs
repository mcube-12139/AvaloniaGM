using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class NewArrayInstruction: IInstruction {
    public readonly IType elementType;
    public readonly int length;

    internal NewArrayInstruction(IType elementType, int length) {
        this.elementType = elementType;
        this.length = length;
    }
}
