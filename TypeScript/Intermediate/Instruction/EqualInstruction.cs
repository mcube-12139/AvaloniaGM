using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class EqualInstruction: IInstruction {
    readonly static Dictionary<IType, EqualInstruction> instantiated = [];
    public IType type;

    EqualInstruction(IType type) {
        this.type = type;
    }

    internal static EqualInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out EqualInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
