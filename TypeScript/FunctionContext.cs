using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript;

internal class FunctionContext(
    ThisParameterSymbol? thisParameterSymbol,
    IType resultType,
    bool returned
) {
    internal readonly ThisParameterSymbol? thisParameterSymbol = thisParameterSymbol;
    internal readonly IType resultType = resultType;
    internal readonly bool returned = returned;
}
