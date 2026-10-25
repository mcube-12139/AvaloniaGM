using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Field;

public class StringLengthField : IField {
    IType IField.GetFieldType() => PrimitiveType.INTEGER;

    void IField.EnsureCanSet(SourcePosition position) {
        throw new SemanticException(SemanticErrorType.NOT_WRITEABLE, ["length"], position);
    }
}
