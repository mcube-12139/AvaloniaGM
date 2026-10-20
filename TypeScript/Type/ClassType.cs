using TypeScript.ExceptionSpace;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class ClassType(string name, ClassType.Field[] fields) : IType {
    public class Field(string name, IType type) {
        public readonly string name = name;
        public readonly IType type = type;
    }

    public class Method(string name, FunctionSymbol symbol) {
        public readonly string name = name;
        public readonly FunctionSymbol symbol = symbol;
    }

    public readonly string name = name;
    public readonly Field[] fields = fields;
    readonly List<Method> methods = [];
    readonly Dictionary<string, Field> fieldDict = new(fields.Select(field => new KeyValuePair<string, Field>(field.name, field)));
    readonly Dictionary<string, Method> methodDict = [];
    
    public bool EqualType(IType other) {
        return other == this;
    }

    public string GetAppearance() => name;

    public Field GetField(string name) => fieldDict[name];

    public Method GetMethod(string name) => methodDict[name];

    void IType.AddMethod(string name, FunctionSymbol symbol, SourcePosition position) {
        Method method = new(name, symbol);
        methods.Add(method);
        methodDict.Add(name, method);
    }

    internal Field InternalGetField(string name, SourcePosition position) {
        if (fieldDict.TryGetValue(name, out Field? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.FIELD_NOT_EXIST, [GetAppearance(), name], position);
    }

    internal Method InternalGetMethod(string name, SourcePosition position) {
        if (methodDict.TryGetValue(name, out Method? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.METHOD_NOT_EXIST, [GetAppearance(), name], position);
    }

    IType IType.GetFieldAccessResultType(string name, SourcePosition position) {
        return InternalGetField(name, position).type;
    }

    FunctionSymbol IType.GetMethodSymbol(string name, SourcePosition position) {
        return InternalGetMethod(name, position).symbol;
    }
}
