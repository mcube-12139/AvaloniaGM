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
    readonly List<ISymbol> implSymbols = [];
    readonly Dictionary<IType, FunctionSymbol[]?> implTypes = [];

    internal ClassType(string name, StructField[] fields) {
        this.name = name;
        this.fields = fields;

        fieldDict = new(fields.Select(field => new KeyValuePair<string, StructField>(field.name, field)));
    }

    public bool EqualType(IType other) {
        return other == this;
    }
    bool IType.IsType(IType other) {
        return other == this || implTypes.ContainsKey(other);
    }

    public string GetAppearance() => name;

    public ISymbol GetSymbol(string name) => space.GetSymbolOrException(name);

    void IType.AddFunctions(IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbolToSpace(space, compiler);
        }
    }

    void IType.AddImpl(ISymbol symbol, IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        implSymbols.Add(symbol);
        if (symbol is InterfaceSymbol interfaceSymbol) {
            implTypes.Add(interfaceSymbol.type, null);
        }
        
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbolToSpace(space, compiler);
        }
    }

    void IType.SetImplMethods(IType type, FunctionSymbol[] methods, SourcePosition position) {
        implTypes[type] = methods;
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
