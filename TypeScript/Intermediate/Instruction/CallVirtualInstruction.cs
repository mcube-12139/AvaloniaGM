namespace TypeScript.Intermediate.Instruction;

public class CallVirtualInstruction: IInstruction {
    public VirtualFunction? fun;

    internal CallVirtualInstruction(VirtualFunction? fun) {
        this.fun = fun;
    }
}
