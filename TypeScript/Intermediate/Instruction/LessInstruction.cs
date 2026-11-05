using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class LessInstruction: IInstruction {
    readonly static Dictionary<IType, LessInstruction> instantiated = [];
    public IType type;

    LessInstruction(IType type) {
        this.type = type;
    }

    internal static LessInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out LessInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
