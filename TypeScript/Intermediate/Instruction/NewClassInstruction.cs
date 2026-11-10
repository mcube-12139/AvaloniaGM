using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class NewClassInstruction: IInstruction {
    public readonly ClassType c;
    public readonly Field[] fields;

    internal NewClassInstruction(ClassType c, Field[] fields) {
        this.c = c;
        this.fields = fields;
    }
}
