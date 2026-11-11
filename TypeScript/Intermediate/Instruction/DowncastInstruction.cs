using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class DowncastInstruction: IInstruction {
    readonly static Dictionary<(IType, IType), DowncastInstruction> instantiated = [];
    public IType source;
    public IType target;

    DowncastInstruction(IType source, IType target) {
        this.source = source;
        this.target = target;
    }

    internal static DowncastInstruction Instantiate(IType source, IType target) {
        if (!instantiated.TryGetValue((source, target), out DowncastInstruction? instruction)) {
            instruction = new(source, target);
            instantiated[(source, target)] = instruction;
        }

        return instruction;
    }
}
