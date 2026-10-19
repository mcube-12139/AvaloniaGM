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
}
