using TypeScript.Type;

namespace TypeScript.Symbol;

public class FunctionSymbol(string name, FunctionType? type) : IValueSymbol {
    internal FunctionType? type = type;

    string ISymbol.GetName() => name;

    IValueSymbol ISymbol.AsValue(TextPosition position, Compiler compiler) {
        return this;
    }

    IType IValueSymbol.GetValueType(TextPosition position, Compiler compiler) {
        return type!;
    }

    internal static FunctionSymbol NewResolved(string name, FunctionType type) =>
        new(name, type);

    internal static FunctionSymbol NewUnresolved(string name) =>
        new(name, null);

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
