using TypeScript.ExceptionSpace;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public interface ITypeSymbol: ISymbol {
    internal IType LazyGetSelfType(SourcePosition position, Compiler compiler);
    IType GetSelfType();
    ITypeSymbol ISymbol.AsType(SourcePosition position) {
        return this;
    }
    void AddImplSymbol(IType? regardedType, FunctionItem[] functions, SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.EXTERNAL_TYPE_IMPLEMENTED, [GetName()], position);
    }
}
