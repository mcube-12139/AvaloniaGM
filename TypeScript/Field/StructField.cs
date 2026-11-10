using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Field;

public class StructField: IField {
    public readonly string name;
    public readonly IType type;

    internal Intermediate.Field intermediateField;

    internal StructField(string name, IType type) {
        this.name = name;
        this.type = type;

        intermediateField = new(name, type.ToIntermediate());
    }

    void IField.EnsureCanSet(SourcePosition position) {
        // 无事发生
    }

    IType IField.GetFieldType() => type;

    void IField.GenerateStore(StatementContext context) {
        context.AddInstruction(StoreFieldInstruction.Instantiate(intermediateField));
    }

    void IField.GenerateLoad(StatementContext context) {
        context.AddInstruction(LoadFieldInstruction.Instantiate(intermediateField));
    }
}
