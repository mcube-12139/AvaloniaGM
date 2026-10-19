using TypeScript.Symbol;

namespace TypeScript;

internal class FlowContext {
    internal bool returned = false;
    internal HashSet<VariableSymbol> initializedSymbols = [];
}
