using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class AddInstruction: IInstruction {
    readonly static Dictionary<IType, AddInstruction> instantiated = [];
    public IType type;

    AddInstruction(IType type) {
        this.type = type;
    }

    internal static AddInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out AddInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
