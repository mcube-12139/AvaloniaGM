using TypeScript.ExceptionSpace;
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

    internal void AddSymbol(string name, ISymbol symbol, SourcePosition position) {
        if (!symbols.TryAdd(name, symbol)) {
            throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], position);
        }
    }

    internal void AddLoop(string name, ILoopStatement statement) {
        if (!loops.TryAdd(name, statement)) {
            throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], statement.GetPosition());
        }
    }

    internal ISymbol? TryGetSymbol(string name) {
        return symbols.GetValueOrDefault(name) ?? null;
    }

    internal ILoopStatement? TryGetLoop(string name) {
        return loops.GetValueOrDefault(name) ?? null;
    }

    internal ISymbol GetSymbol(string name, SourcePosition position) {
        if (symbols.TryGetValue(name, out ISymbol? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [name], position);
    }

    internal ISymbol GetSymbolOrException(string name) {
        return symbols[name];
    }
}
