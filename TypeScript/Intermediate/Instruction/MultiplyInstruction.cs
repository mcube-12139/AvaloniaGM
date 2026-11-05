using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class MultiplyInstruction: IInstruction {
    readonly static Dictionary<IType, MultiplyInstruction> instantiated = [];
    public IType type;

    MultiplyInstruction(IType type) {
        this.type = type;
    }

    internal static MultiplyInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out MultiplyInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
