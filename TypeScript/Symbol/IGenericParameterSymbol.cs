using TypeScript.GenericArgument;

namespace TypeScript.Symbol;

public interface IGenericParameterSymbol: ISymbol {
    internal void SetArgument(IGenericArgument argument, SourcePosition position);
    void SetArgumentOrThrow(IGenericArgument argument);
}
