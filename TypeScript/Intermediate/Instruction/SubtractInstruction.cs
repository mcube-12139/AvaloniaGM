using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class SubtractInstruction: IInstruction {
    readonly static Dictionary<IType, SubtractInstruction> instantiated = [];
    public IType type;

    SubtractInstruction(IType type) {
        this.type = type;
    }

    internal static SubtractInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out SubtractInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
