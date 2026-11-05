using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class GreaterInstruction: IInstruction {
    readonly static Dictionary<IType, GreaterInstruction> instantiated = [];
    public IType type;

    GreaterInstruction(IType type) {
        this.type = type;
    }

    internal static GreaterInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out GreaterInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
