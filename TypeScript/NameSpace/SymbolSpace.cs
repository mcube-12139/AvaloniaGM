using TypeScript.ExceptionSpace;
using TypeScript.Statement;
using TypeScript.Symbol;

namespace TypeScript.NameSpace;

internal class SymbolSpace: INameSpace {
    readonly internal INameSpace? upper;
    readonly Dictionary<string, ISymbol> symbols = [];

    INameSpace? INameSpace.GetUpper() => upper;

    internal SymbolSpace(INameSpace? upper) {
        this.upper = upper;
    }

    internal void Clear() {
        symbols.Clear();
    }

    void INameSpace.AddSymbol(string name, ISymbol symbol, SourcePosition position) => AddSymbol(name, symbol, position);
    ISymbol? INameSpace.TryGetSymbol(string name) => TryGetSymbol(name);
    ISymbol INameSpace.GetSymbol(string name, SourcePosition position) => GetSymbol(name, position);
    ISymbol INameSpace.GetSymbolOrException(string name) => GetSymbolOrException(name);

    internal void AddSymbol(string name, ISymbol symbol, SourcePosition position) {
        if (!symbols.TryAdd(name, symbol)) {
            throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], position);
        }
    }

    internal ISymbol? TryGetSymbol(string name) {
        return symbols.GetValueOrDefault(name) ?? null;
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
