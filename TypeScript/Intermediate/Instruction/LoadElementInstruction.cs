using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class LoadElementInstruction : IInstruction {
    readonly static Dictionary<IType, LoadElementInstruction> instantiated = [];
    public IType type;

    LoadElementInstruction(IType type) {
        this.type = type;
    }

    internal static LoadElementInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out LoadElementInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
