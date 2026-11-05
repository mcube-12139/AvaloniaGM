namespace TypeScript.Intermediate.Instruction;

public class StoreArgumentInstruction: IInstruction {
    public readonly Parameter parameter;

    internal StoreArgumentInstruction(Parameter parameter) {
        this.parameter = parameter;
    }
}
