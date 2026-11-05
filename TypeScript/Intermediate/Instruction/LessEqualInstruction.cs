using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class LessEqualInstruction: IInstruction {
    readonly static Dictionary<IType, LessEqualInstruction> instantiated = [];
    public IType type;

    LessEqualInstruction(IType type) {
        this.type = type;
    }

    internal static LessEqualInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out LessEqualInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
