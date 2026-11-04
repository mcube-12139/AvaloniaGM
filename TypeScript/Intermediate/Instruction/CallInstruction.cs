namespace TypeScript.Intermediate.Instruction;

public class CallInstruction: IInstruction {
    public Function? fun;

    internal CallInstruction(Function? fun) {
        this.fun = fun;
    }
}
