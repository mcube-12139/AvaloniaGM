using TypeScript.Symbol;

namespace TypeScript.FlowContext;

internal class FlowBranchContext {
    internal bool returned = false;
    internal HashSet<LocalSymbol> initializedSymbols = [];
}
