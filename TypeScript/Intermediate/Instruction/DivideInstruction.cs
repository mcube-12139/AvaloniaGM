using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class DivideInstruction: IInstruction {
    readonly static Dictionary<IType, DivideInstruction> instantiated = [];
    public IType type;

    DivideInstruction(IType type) {
        this.type = type;
    }

    internal static DivideInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out DivideInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
