using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class FunctionSymbol(string name, FunctionType? type, FunctionItem? item, string? hostName) : IValueSymbol {
    public readonly string name = name;
    internal FunctionType? type = type;
    internal FunctionItem? item = item;
    public readonly string? hostName = hostName;

    string ISymbol.GetName() => name;

    IValueSymbol ISymbol.AsValue(TextPosition position, Compiler compiler) {
        return this;
    }

    IType IValueSymbol.LazyGetValueType(TextPosition position, Compiler compiler) {
        // 不能调用 SetSemantic，会无限递归
        type ??= item!.ResolveType(compiler);
        return type;
    }

    IType IValueSymbol.GetValueType() => type!;

    public FunctionType GetFunctionType() => type!;

    internal static FunctionSymbol NewResolved(string name, FunctionType type, string? hostName) =>
        new(name, type, null, hostName);

    internal static FunctionSymbol NewUnresolved(string name, FunctionItem item, string? hostName) =>
        new(name, null, item, hostName);

    /*
    void Resolve(Generator generator) {
        type ??= item!.ResolveType(generator);
    }

    ITypeSymbol ISymbol.AsType(TextPosition position, Generator generator) {
        throw generator.SemanticError(SemanticErrorType.NOT_TYPE, [name], position);
    }

    void IValueSymbol.Call(TextPosition position, Generator generator) {
        Resolve(generator);
        type!.Call(fun, generator);
    }

    void IValueSymbol.Load(TextPosition position, Generator generator) {
        throw new System.NotImplementedException();
    }

    void IValueSymbol.Store(TextPosition position, Generator generator) {
        throw generator.SemanticError(SemanticErrorType.NOT_WRITEABLE, [name], position);
    }

    void IValueSymbol.StoreIndex(IExpression right, IExpression index, TextPosition position, Generator generator) {
        throw new System.NotImplementedException();
    }

    void IValueSymbol.LoadIndex(IExpression index, TextPosition position, Generator generator) {
        throw new System.NotImplementedException();
    }
    */
}
