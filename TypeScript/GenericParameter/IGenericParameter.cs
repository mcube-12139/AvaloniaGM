using TypeScript.Symbol;

namespace TypeScript.GenericParameter;

public interface IGenericParameter {
    internal IGenericParameterSymbol AddSymbol(Compiler compiler);
}
