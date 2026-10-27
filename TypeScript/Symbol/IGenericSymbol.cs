using TypeScript.GenericArgument;

namespace TypeScript.Symbol;

public interface IGenericSymbol: ISymbol {
    IGenericSymbol ISymbol.AsGeneric(SourcePosition position) => this;
    ISymbol Monomorphize(IGenericArgument[] arguments, SourcePosition position);
}
