using TypeScript.ExceptionSpace;
using TypeScript.Field;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class ClassType : IType {
    readonly TypeHead head;
    public readonly string name;
    public readonly StructField[] fields;

    readonly Dictionary<string, StructField> fieldDict;
    readonly Dictionary<IType, FunctionSymbol[]?> implTypes = [];

    internal ClassType(string name, StructField[] fields) {
        head = new();
        this.name = name;
        this.fields = fields;

        fieldDict = new(fields.Select(field => new KeyValuePair<string, StructField>(field.name, field)));
    }

    TypeHead IType.GetHead() => head;

    bool IType.IsType(IType other) {
        return other == this || implTypes.ContainsKey(other);
    }

    public string GetAppearance() => name;

    public ISymbol GetSymbol(string name) => head.space.GetSymbolOrException(name);

    internal StructField GetField(string name, SourcePosition position) {
        if (fieldDict.TryGetValue(name, out StructField? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [GetAppearance(), name], position);
    }

    IField IType.GetField(string name, SourcePosition position) => GetField(name, position);
}
