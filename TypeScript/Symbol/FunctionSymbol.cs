using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class FunctionSymbol : IValueSymbol {
    public readonly string name;
    internal readonly NameSpace itemSpace;
    public readonly bool method;
    readonly FunctionItem? item;
    public readonly string? hostName;

    internal FunctionType? type;

    internal FunctionSymbol(string name, NameSpace itemSpace, bool method, FunctionItem? item, string? hostName) {
        this.name = name;
        this.itemSpace = itemSpace;
        this.method = method;
        this.item = item;
        this.hostName = hostName;
    }

    string ISymbol.GetName() => name;

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => LazyGetValueType(position, compiler);

    internal FunctionType LazyGetValueType(SourcePosition position, Compiler compiler) {
        // 不能调用 SetSemantic，会无限递归
        if (type == null) {
            compiler.PushNameSpace(itemSpace);
            item!.ResolveType(compiler);
            compiler.PopNameSpace();
        }
        return type!;
    }

    IType IValueSymbol.GetValueType() => type!;

    public FunctionType GetFunctionType() => type!;

    IType ISymbol.GetMethodCallResultType(IExpression[] parameterTypes, SourcePosition position, Compiler compiler) {
        if (!method) {
            throw new SemanticException(SemanticErrorType.NOT_METHOD, [name], position);
        }

        var funType = LazyGetValueType(position, compiler);
        funType.EnsureCanCallMethod(parameterTypes, position, compiler);
        return funType.resultType;
    }

    void IValueSymbol.Store(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_WRITEABLE, [name], position);
    }

    void IValueSymbol.EnsureCanLoad(SourcePosition position) {
        // 无事发生
    }
}
