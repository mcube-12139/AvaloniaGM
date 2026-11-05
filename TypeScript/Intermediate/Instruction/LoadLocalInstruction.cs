namespace TypeScript.Intermediate.Instruction;

public class LoadLocalInstruction: IInstruction {
    public int index;

    internal LoadLocalInstruction(int index) {
        this.index = index;
    }
}
