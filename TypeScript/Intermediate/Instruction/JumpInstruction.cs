namespace TypeScript.Intermediate.Instruction;

public class JumpInstruction: IInstruction {
    public int index;

    internal JumpInstruction(int index) {
        this.index = index;
    }

    internal static JumpInstruction Unresolved() {
        return new(0);
    }
}
