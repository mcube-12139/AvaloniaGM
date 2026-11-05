using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class NotEqualInstruction: IInstruction {
    readonly static Dictionary<IType, NotEqualInstruction> instantiated = [];
    public IType type;

    NotEqualInstruction(IType type) {
        this.type = type;
    }

    internal static NotEqualInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out NotEqualInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
