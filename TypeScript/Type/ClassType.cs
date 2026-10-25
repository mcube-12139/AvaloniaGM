using TypeScript.ExceptionSpace;
using TypeScript.Field;
using TypeScript.Item;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class ClassType(string name, StructField[] fields) : IType {
    public readonly string name = name;
    public readonly StructField[] fields = fields;
    readonly Dictionary<string, StructField> fieldDict = new(fields.Select(field => new KeyValuePair<string, StructField>(field.name, field)));
    internal readonly NameSpace space = new();
    readonly List<ISymbol> implSymbols = [];
    readonly Dictionary<IType, FunctionSymbol[]?> implTypes = [];
    
    public bool EqualType(IType other) {
        return other == this;
    }
    bool IType.IsType(IType other) {
        return other == this || implTypes.ContainsKey(other);
    }

    public string GetAppearance() => name;

    public ISymbol GetSymbol(string name) => space.GetSymbolOrException(name);

    void IType.AddFunctions(IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        compiler.EnterNameSpace(space);
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbol(compiler);
        }
        compiler.LeaveNameSpace();
    }

    void IType.AddImpl(ISymbol symbol, IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        implSymbols.Add(symbol);
        if (symbol is InterfaceSymbol interfaceSymbol) {
            implTypes.Add(interfaceSymbol.type, null);
        }
        
        compiler.EnterNameSpace(space);
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbol(compiler);
        }
        compiler.LeaveNameSpace();
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
