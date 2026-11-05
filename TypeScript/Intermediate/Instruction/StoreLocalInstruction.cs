namespace TypeScript.Intermediate.Instruction;

public class StoreLocalInstruction: IInstruction {
    public int index;

    internal StoreLocalInstruction(int index) {
        this.index = index;
    }
}
