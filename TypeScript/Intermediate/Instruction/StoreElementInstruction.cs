using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class StoreElementInstruction : IInstruction {
    readonly static Dictionary<IType, StoreElementInstruction> instantiated = [];
    public IType type;

    StoreElementInstruction(IType type) {
        this.type = type;
    }

    internal static StoreElementInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out StoreElementInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
