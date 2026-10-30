using TypeScript.GenericArgument;

namespace TypeScript.Symbol;

public interface IGenericParameterSymbol: ISymbol {
    internal void SetArgument(IGenericArgument argument, SourcePosition position);
    internal IGenericArgument SetInferredArgument(GenericInferContext context);
    void SetArgumentOrThrow(IGenericArgument argument);
}
