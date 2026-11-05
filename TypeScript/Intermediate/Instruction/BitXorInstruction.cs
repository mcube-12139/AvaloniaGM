using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class BitXorInstruction: IInstruction {
    readonly static Dictionary<IType, BitXorInstruction> instantiated = [];
    public IType type;

    BitXorInstruction(IType type) {
        this.type = type;
    }

    internal static BitXorInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out BitXorInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
