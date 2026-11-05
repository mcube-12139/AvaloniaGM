using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class GreaterEqualInstruction: IInstruction {
    readonly static Dictionary<IType, GreaterEqualInstruction> instantiated = [];
    public IType type;

    GreaterEqualInstruction(IType type) {
        this.type = type;
    }

    internal static GreaterEqualInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out GreaterEqualInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
