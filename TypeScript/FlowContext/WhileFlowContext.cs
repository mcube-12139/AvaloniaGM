using TypeScript.Symbol;

namespace TypeScript.FlowContext;

internal class WhileFlowContext : IFlowContext {
    readonly HashSet<LocalSymbol> resetLocals = [];

    void IFlowContext.AddInitializedLocal(LocalSymbol symbol) {
        resetLocals.Add(symbol);
    }

    bool IFlowContext.GetReturned() => false;

    IEnumerable<LocalSymbol> IFlowContext.IterateInitializedLocals() => [];

    IEnumerable<LocalSymbol> IFlowContext.IterateResetLocals() => resetLocals;

    void IFlowContext.Leave() { }

    void IFlowContext.SetReturned() { }
}
