namespace TypeScript.Intermediate.Instruction;

public class ConstFunctionInstruction: IInstruction {
    public Function? fun;

    internal ConstFunctionInstruction(Function? fun) {
        this.fun = fun;
    }
}
