namespace TypeScript.Intermediate.Instruction;

public class StoreFieldInstruction : IInstruction {
    readonly static Dictionary<Field, StoreFieldInstruction> instantiated = [];
    public readonly Field field;

    StoreFieldInstruction(Field field) {
        this.field = field;
    }

    internal static StoreFieldInstruction Instantiate(Field field) {
        if (!instantiated.TryGetValue(field, out StoreFieldInstruction? instruction)) {
            instruction = new(field);
            instantiated[field] = instruction;
        }

        return instruction;
    }
}
