using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class BitAndInstruction: IInstruction {
    readonly static Dictionary<IType, BitAndInstruction> instantiated = [];
    public IType type;

    BitAndInstruction(IType type) {
        this.type = type;
    }

    internal static BitAndInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out BitAndInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
