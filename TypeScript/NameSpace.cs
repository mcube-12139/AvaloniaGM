using TypeScript.Statement;
using TypeScript.Symbol;

namespace TypeScript;

internal class NameSpace {
    readonly Dictionary<string, ISymbol> symbols = [];
    readonly Dictionary<string, ILoopStatement> loops = [];

    internal void Clear() {
        symbols.Clear();
        loops.Clear();
    }

    internal bool TryAddSymbol(string name, ISymbol symbol) {
        return symbols.TryAdd(name, symbol);
    }

    internal bool TryAddLoop(string name, ILoopStatement statement) {
        return loops.TryAdd(name, statement);
    }

    internal ISymbol? GetSymbol(string name) {
        return symbols.GetValueOrDefault(name) ?? null;
    }

    internal ILoopStatement? GetLoop(string name) {
        return loops.GetValueOrDefault(name) ?? null;
    }
}
