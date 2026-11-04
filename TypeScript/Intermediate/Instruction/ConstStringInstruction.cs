namespace TypeScript.Intermediate.Instruction;

public class ConstStringInstruction: IInstruction {
    public readonly string value;

    internal ConstStringInstruction(string value) {
        this.value = value;
    }
}
