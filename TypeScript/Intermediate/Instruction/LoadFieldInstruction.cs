namespace TypeScript.Intermediate.Instruction;

public class LoadFieldInstruction : IInstruction {
    readonly static Dictionary<Field, LoadFieldInstruction> instantiated = [];
    public readonly Field field;

    LoadFieldInstruction(Field field) {
        this.field = field;
    }

    internal static LoadFieldInstruction Instantiate(Field field) {
        if (!instantiated.TryGetValue(field, out LoadFieldInstruction? instruction)) {
            instruction = new(field);
            instantiated[field] = instruction;
        }

        return instruction;
    }
}
