using TypeScript.ExceptionSpace;
using TypeScript.Item;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class ClassType(string name, ClassType.Field[] fields) : IType {
    public class Field(string name, IType type) {
        public readonly string name = name;
        public readonly IType type = type;
    }

    public class Function(string name, FunctionSymbol symbol) {
        public readonly string name = name;
        public readonly FunctionSymbol symbol = symbol;
    }

    public readonly string name = name;
    public readonly Field[] fields = fields;
    readonly List<Function> functions = [];
    readonly Dictionary<string, Field> fieldDict = new(fields.Select(field => new KeyValuePair<string, Field>(field.name, field)));
    internal readonly NameSpace space = new();
    readonly List<ISymbol> implSymbols = [];
    readonly HashSet<InterfaceType> implTypes = [];
    
    public bool EqualType(IType other) {
        return other == this;
    }
    bool IType.IsType(IType other) {
        return other == this || implTypes.Contains(other);
    }

    public string GetAppearance() => name;

    public Field GetField(string name) => fieldDict[name];

    public ISymbol GetSymbol(string name) => space.GetSymbolOrException(name);

    void IType.AddFunctions(IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        compiler.EnterNameSpace(space);
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbol(compiler);
            this.functions.Add(new(funItem.name, funItem.GetSymbol()));
        }
        compiler.LeaveNameSpace();
    }

    void IType.AddImpl(ISymbol symbol, IEnumerable<FunctionItem> functions, SourcePosition position, Compiler compiler) {
        implSymbols.Add(symbol);
        if (symbol is InterfaceSymbol interfaceSymbol) {
            implTypes.Add(interfaceSymbol.type);
        }
        
        compiler.EnterNameSpace(space);
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbol(compiler);
            this.functions.Add(new(funItem.name, funItem.GetSymbol()));
        }
        compiler.LeaveNameSpace();
    }

    internal Field InternalGetField(string name, SourcePosition position) {
        if (fieldDict.TryGetValue(name, out Field? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [GetAppearance(), name], position);
    }

    IType IType.GetFieldAccessResultType(string name, SourcePosition position) {
        return InternalGetField(name, position).type;
    }

    ISymbol IType.GetImplItemSymbol(string name, SourcePosition position) {
        return space.GetSymbol(name, position);
    }
}
