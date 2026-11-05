using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class LeftShiftInstruction: IInstruction {
    readonly static Dictionary<IType, LeftShiftInstruction> instantiated = [];
    public IType type;

    LeftShiftInstruction(IType type) {
        this.type = type;
    }

    internal static LeftShiftInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out LeftShiftInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
