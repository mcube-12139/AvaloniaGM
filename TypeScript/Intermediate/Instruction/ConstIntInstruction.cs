namespace TypeScript.Intermediate.Instruction;

public class ConstIntInstruction: IInstruction {
    public readonly int value;

    internal ConstIntInstruction(int value) {
        this.value = value;
    }
}
