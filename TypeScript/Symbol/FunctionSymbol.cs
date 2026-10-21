using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class FunctionSymbol : IValueSymbol {
    public readonly string name;
    public readonly bool method;
    internal FunctionType? type;
    readonly FunctionItem? item;
    public readonly string? hostName;

    FunctionSymbol(string name, bool method, FunctionType? type, FunctionItem? item, string? hostName) {
        this.name = name;
        this.method = method;
        this.type = type;
        this.item = item;
        this.hostName = hostName;
    }

    string ISymbol.GetName() => name;

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => LazyGetValueType(position, compiler);

    internal FunctionType LazyGetValueType(SourcePosition position, Compiler compiler) {
        // 不能调用 SetSemantic，会无限递归
        type ??= item!.ResolveType(compiler);
        return type;
    }

    IType IValueSymbol.GetValueType() => type!;

    public FunctionType GetFunctionType() => type!;

    internal static FunctionSymbol NewResolved(string name, bool method, FunctionType type, string? hostName) =>
        new(name, method, type, null, hostName);

    internal static FunctionSymbol NewUnresolved(string name, bool method, FunctionItem item, string? hostName) =>
        new(name, method, null, item, hostName);

    IType IValueSymbol.GetMethodCallResultType(IExpression[] parameterTypes, SourcePosition position, Compiler compiler) {
        if (!method) {
            throw new SemanticException(SemanticErrorType.NOT_METHOD, [name], position);
        }

        var funType = LazyGetValueType(position, compiler);
        funType.EnsureCanCallMethod(parameterTypes, position, compiler);
        return funType.resultType;
    }
}
