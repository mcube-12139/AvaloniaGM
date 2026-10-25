using TypeScript.Type;

namespace TypeScript.Field;

public class StructField: IField {
    public readonly string name;
    public readonly IType type;

    internal StructField(string name, IType type) {
        this.name = name;
        this.type = type;
    }

    void IField.EnsureCanSet(SourcePosition position) {
        // 无事发生
    }

    IType IField.GetFieldType() => type;
}
