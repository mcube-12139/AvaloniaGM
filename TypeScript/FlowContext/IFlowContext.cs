using TypeScript.Symbol;

namespace TypeScript.FlowContext;

internal interface IFlowContext {
    void Leave();
    void SetReturned();
    bool GetReturned();
    void AddInitializedLocal(LocalSymbol symbol);
    /// <summary>
    /// 穷举在本环境中被初始化的符号集
    /// </summary>
    IEnumerable<LocalSymbol> IterateInitializedLocals();
    IEnumerable<LocalSymbol> IterateResetLocals();
}
