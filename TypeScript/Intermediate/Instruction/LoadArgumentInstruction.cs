namespace TypeScript.Intermediate.Instruction;

public class LoadArgumentInstruction: IInstruction {
    public readonly Parameter parameter;

    internal LoadArgumentInstruction(Parameter parameter) {
        this.parameter = parameter;
    }
}
