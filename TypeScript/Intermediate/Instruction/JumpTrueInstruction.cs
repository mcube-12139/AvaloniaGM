namespace TypeScript.Intermediate.Instruction;

public class JumpTrueInstruction: IInstruction {
    public int index;

    internal JumpTrueInstruction(int index) {
        this.index = index;
    }

    internal static JumpTrueInstruction Unresolved() {
        return new(0);
    }
}
