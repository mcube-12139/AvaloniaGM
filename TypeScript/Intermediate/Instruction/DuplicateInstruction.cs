using System.Collections.Immutable;
using TypeScript.Intermediate.Type;

namespace TypeScript.Intermediate.Instruction;

public class DuplicateInstruction: IInstruction {
    readonly static Dictionary<ImmutableArray<IType>, DuplicateInstruction> instantiated = new(TypeArrayComparer.COMPARER);
    public readonly ImmutableArray<IType> types;

    DuplicateInstruction(ImmutableArray<IType> types) {
        this.types = types;
    }

    internal static DuplicateInstruction Instantiate(ImmutableArray<IType> elementTypes) {
        if (!instantiated.TryGetValue(elementTypes, out DuplicateInstruction? result)) {
            result = new(elementTypes);
            instantiated[elementTypes] = result;
        }

        return result;
    }
}
