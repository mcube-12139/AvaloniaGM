using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class CallPointerInstruction: IInstruction {
    public FunctionType type;

    internal CallPointerInstruction(FunctionType type) {
        this.type = type;
    }
}
