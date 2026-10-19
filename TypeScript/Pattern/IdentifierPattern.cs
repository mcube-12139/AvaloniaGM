using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Pattern;

public class IdentifierPattern(SourcePosition position, string name) : IPattern {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    ISymbol? symbol;

    public ISymbol GetSymbol() => symbol!;

    void IPattern.AddLocal(IType type, bool initialized, Compiler compiler) {
        symbol = new LocalSymbol(name, type, initialized);
        compiler.AddSymbol(name, symbol, position);
    }

    void IPattern.AddParameter(IType type, Compiler compiler) {
        symbol = new ParameterSymbol(name, type);
        compiler.AddSymbol(name, symbol, position);
    }
}
