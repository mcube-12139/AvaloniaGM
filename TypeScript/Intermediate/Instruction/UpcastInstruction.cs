using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class UpcastInstruction: IInstruction {
    readonly static Dictionary<(IType, IType), UpcastInstruction> instantiated = [];
    public IType source;
    public IType target;

    UpcastInstruction(IType source, IType target) {
        this.source = source;
        this.target = target;
    }

    internal static UpcastInstruction Instantiate(IType source, IType target) {
        if (!instantiated.TryGetValue((source, target), out UpcastInstruction? instruction)) {
            instruction = new(source, target);
            instantiated[(source, target)] = instruction;
        }

        return instruction;
    }
}
