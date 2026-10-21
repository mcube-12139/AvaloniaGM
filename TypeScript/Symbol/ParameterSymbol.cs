using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ParameterSymbol(
    string name,
    IType type
) : IValueSymbol {
    public readonly string name = name;
    public readonly IType type = type;

    string ISymbol.GetName() => name;

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => type;

    IType IValueSymbol.GetValueType() => type;

    void IValueSymbol.Store(SourcePosition position, Compiler compiler) {
        // 无事发生
    }

    void IValueSymbol.EnsureCanLoad(SourcePosition position) {
        // 无事发生
    }
}
