using TypeScript.ExceptionSpace;
using TypeScript.Type;

namespace TypeScript.Symbol;

/// <summary>
/// 形如 T::someFun，非泛型
/// </summary>
public class ConceptFunctionSymbol : IValueSymbol {
    public readonly string name;
    public readonly FunctionType type;
    public readonly GenericType genericType;
    public readonly IConceptSymbol conceptSymbol;

    string ISymbol.GetName() => name;
    IType IValueSymbol.GetValueType() => type;
    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => type;

    internal ConceptFunctionSymbol(string name, FunctionType type, GenericType genericType, IConceptSymbol conceptSymbol) {
        this.name = name;
        this.type = type;
        this.genericType = genericType;
        this.conceptSymbol = conceptSymbol;
    }

    void IValueSymbol.Store(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_WRITEABLE, [name], position);
    }

    void IValueSymbol.EnsureCanLoad(SourcePosition position) {
        // 无事发生
    }
}
