using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class LocalSymbol(
    string name,
    IType type,
    bool initialized
) : IValueSymbol {
    public readonly string name = name;
    public readonly IType type = type;

    internal bool initialized = initialized;

    string ISymbol.GetName() => name;

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => type;

    IType IValueSymbol.GetValueType() => type;

    void IValueSymbol.Store(SourcePosition position, Compiler compiler) => Store(position, compiler);

    internal void Store(SourcePosition position, Compiler compiler) {
        if (!initialized) {
            initialized = true;
            compiler.AddInitializedLocal(this);
        }
    }

    void IValueSymbol.EnsureCanLoad(SourcePosition position) {
        if (!initialized) {
            throw new SemanticException(SemanticErrorType.MAY_NOT_INITIALIZED, [name], position);
        }
    }
}
