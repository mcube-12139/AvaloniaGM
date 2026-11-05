using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class BitOrInstruction: IInstruction {
    readonly static Dictionary<IType, BitOrInstruction> instantiated = [];
    public IType type;

    BitOrInstruction(IType type) {
        this.type = type;
    }

    internal static BitOrInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out BitOrInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
