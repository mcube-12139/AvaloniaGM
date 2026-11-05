namespace TypeScript.Intermediate.Instruction;

public class JumpFalseInstruction: IInstruction {
    public int index;

    internal JumpFalseInstruction(int index) {
        this.index = index;
    }

    internal static JumpFalseInstruction Unresolved() {
        return new(0);
    }
}
