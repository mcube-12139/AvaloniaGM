using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class ModuloInstruction: IInstruction {
    readonly static Dictionary<IType, ModuloInstruction> instantiated = [];
    public IType type;

    ModuloInstruction(IType type) {
        this.type = type;
    }

    internal static ModuloInstruction Instantiate(IType type) {
        if (!instantiated.TryGetValue(type, out ModuloInstruction? instruction)) {
            instruction = new(type);
            instantiated[type] = instruction;
        }

        return instruction;
    }
}
