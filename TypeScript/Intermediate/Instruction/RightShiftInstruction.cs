using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class RightShiftInstruction: IInstruction {
    readonly static Dictionary<IType, RightShiftInstruction> instantiated = [];
    public IType type;

    RightShiftInstruction(IType type) {
        this.type = type;
    }

    internal static RightShiftInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out RightShiftInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
