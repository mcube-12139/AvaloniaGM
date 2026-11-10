using TypeScript.ExceptionSpace;
using TypeScript.Field;
using TypeScript.Item;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class ClassType : IType {
    public readonly string name;
    public readonly StructField[] fields;

    readonly Dictionary<string, StructField> fieldDict;
    internal readonly NameSpace space = new(null);
    readonly HashSet<IType> implTypes = [];
    readonly Dictionary<IConceptSymbol, FunctionSymbol[]?> implConcepts = [];

    internal Intermediate.Type.ClassType intermediateClass;

    Intermediate.Type.IType IType.ToIntermediate() => intermediateClass!;

    internal ClassType(string name, StructField[] fields) {
        this.name = name;
        this.fields = fields;

        fieldDict = new(fields.Select(field => new KeyValuePair<string, StructField>(field.name, field)));
        intermediateClass = new(name, [.. fields.Select(field => field.intermediateField)]);
    }

    public bool EqualType(IType other) {
        return other == this;
    }
    bool IType.IsType(IType other) {
        return other == this || implTypes.Contains(other);
    }

    public string GetAppearance() => name;

    public ISymbol GetSymbol(string name) => space.GetSymbolOrException(name);

    void IType.AddFunctions(IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbolToSpace(space, compiler);
        }
    }

    void IType.AddImpl(IConceptSymbol symbol, IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        if (symbol is InterfaceSymbol interfaceSymbol) {
            implTypes.Add(interfaceSymbol.type);
        }
        
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbolToSpace(space, compiler);
        }
    }

    void IType.SetImplMethods(IConceptSymbol conceptSymbol, FunctionSymbol[] methods, SourcePosition position) {
        implConcepts[conceptSymbol] = methods;
    }

    internal StructField InternalGetField(string name, SourcePosition position) {
        if (fieldDict.TryGetValue(name, out StructField? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [GetAppearance(), name], position);
    }

    IField IType.GetField(string name, SourcePosition position) {
        return InternalGetField(name, position);
    }

    ISymbol IType.GetAssociatedSymbol(string name, SourcePosition position) {
        return space.GetSymbol(name, position);
    }
}
