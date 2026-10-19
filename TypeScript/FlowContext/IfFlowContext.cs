using TypeScript.Symbol;

namespace TypeScript.FlowContext;

internal class IfFlowContext: IFlowContext {
    readonly FlowBranchContext ifContext = new();
    readonly FlowBranchContext elseContext = new();
    FlowBranchContext nowContext;

    bool returned;
    readonly HashSet<LocalSymbol> initializedLocals = [];
    readonly HashSet<LocalSymbol> resetLocals = [];

    internal IfFlowContext() {
        ifContext = new();
        elseContext = new();
        nowContext = ifContext;
    }

    internal void StartElse() {
        nowContext = elseContext;
    }

    void IFlowContext.AddInitializedLocal(LocalSymbol symbol) {
        nowContext.initializedSymbols.Add(symbol);
    }

    void IFlowContext.SetReturned() {
        nowContext.returned = true;
    }

    IEnumerable<LocalSymbol> IFlowContext.IterateInitializedLocals() => initializedLocals;

    IEnumerable<LocalSymbol> IFlowContext.IterateResetLocals() => resetLocals;

    bool IFlowContext.GetReturned() => returned;

    void IFlowContext.Leave() {
        if (ifContext.returned && elseContext.returned) {
            returned = true;
        }

        if (!ifContext.returned && !elseContext.returned) {
            // 两边都不返回，把交集符号加入上一层，其他符号改回未初始化
            HashSet<LocalSymbol> elseLeftSymbols = [];
            foreach (LocalSymbol elseSymbol in elseContext.initializedSymbols) {
                elseLeftSymbols.Add(elseSymbol);
            }
            foreach (LocalSymbol ifSymbol in ifContext.initializedSymbols) {
                if (elseLeftSymbols.Contains(ifSymbol)) {
                    initializedLocals.Add(ifSymbol);
                    elseLeftSymbols.Remove(ifSymbol);
                } else {
                    resetLocals.Add(ifSymbol);
                }
            }
            foreach (LocalSymbol symbol in elseLeftSymbols) {
                resetLocals.Add(symbol);
            }
        } else {
            // 有返回的，把没返回的那边的符号加入上一层
            if (ifContext.returned) {
                foreach (LocalSymbol elseSymbol in elseContext.initializedSymbols) {
                    initializedLocals.Add(elseSymbol);
                }
            } else if (elseContext.returned) {
                foreach (LocalSymbol ifSymbol in ifContext.initializedSymbols) {
                    resetLocals.Add(ifSymbol);
                }
            }
        }
    }
}
