using TypeScript.ExceptionSpace;
using TypeScript.Statement;
using TypeScript.Symbol;

namespace TypeScript.NameSpace;

internal class BlockSpace: INameSpace {
    readonly internal INameSpace? upper;
    readonly internal BlockSpace? upperBlock;
    readonly Dictionary<string, ISymbol> symbols = [];
    readonly Dictionary<string, ILoopStatement> loops = [];

    INameSpace? INameSpace.GetUpper() => upper;

    internal BlockSpace(INameSpace? upper, BlockSpace? upperBlock) {
        this.upper = upper;
        this.upperBlock = upperBlock;
    }

    internal void Clear() {
        symbols.Clear();
        loops.Clear();
    }

    void INameSpace.AddSymbol(string name, ISymbol symbol, SourcePosition position) {
        if (!symbols.TryAdd(name, symbol)) {
            throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], position);
        }
    }

    internal void AddLoop(string name, ILoopStatement statement) {
        if (!loops.TryAdd(name, statement)) {
            throw new SemanticException(SemanticErrorType.SYMBOL_EXIST, [name], statement.GetPosition());
        }
    }

    ISymbol? INameSpace.TryGetSymbol(string name) {
        return symbols.GetValueOrDefault(name) ?? null;
    }

    internal ILoopStatement? TryGetLoop(string name) {
        return loops.GetValueOrDefault(name) ?? null;
    }

    ISymbol INameSpace.GetSymbol(string name, SourcePosition position) {
        if (symbols.TryGetValue(name, out ISymbol? result)) {
            return result;
        }

        throw new SemanticException(SemanticErrorType.SYMBOL_NOT_EXIST, [name], position);
    }

    ISymbol INameSpace.GetSymbolOrException(string name) {
        return symbols[name];
    }
}
