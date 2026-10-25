using TypeScript.Type;

namespace TypeScript.Field;

public interface IField {
    IType GetFieldType();
    void EnsureCanSet(SourcePosition position);
}
