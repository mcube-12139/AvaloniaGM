using TypeScript.Symbol;

namespace TypeScript;

internal class FlowContext {
    internal bool returned = false;
    /// <summary>
    /// 在本环境中变成已初始化的符号集
    /// </summary>
    internal HashSet<VariableSymbol> initializedSymbols = [];
}
