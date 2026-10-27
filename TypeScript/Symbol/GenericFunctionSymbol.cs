using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.GenericArgument;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class GenericFunctionSymbol : IGenericSymbol {
    public readonly string name;
    internal readonly NameSpace itemSpace;
    public readonly bool method;
    readonly FunctionItem? item;

    internal FunctionType? type;

    internal GenericFunctionSymbol(string name, NameSpace itemSpace, bool method, FunctionItem? item) {
        this.name = name;
        this.itemSpace = itemSpace;
        this.method = method;
        this.item = item;
    }

    string ISymbol.GetName() => name;

    ISymbol IGenericSymbol.Monomorphize(IGenericArgument[] arguments, SourcePosition position) {
        throw new NotImplementedException();
    }
}
