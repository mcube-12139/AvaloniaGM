using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class ReturnInstruction: IInstruction {
    readonly static Dictionary<IType, ReturnInstruction> instantiated = [];
    public IType type;

    ReturnInstruction(IType type) {
        this.type = type;
    }

    internal static ReturnInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out ReturnInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
