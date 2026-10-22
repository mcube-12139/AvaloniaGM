using System.Collections.Immutable;
using TypeScript.Symbol;

namespace TypeScript.Item;

public class InterfaceItem : IItem {
    public readonly SourcePosition position;
    public readonly string name;
    public readonly ImmutableArray<FunctionItem> functions;

    InterfaceSymbol? symbol;

    internal InterfaceItem(SourcePosition position, string name, ImmutableArray<FunctionItem> functions) {
        this.position = position;
        this.name = name;
        this.functions = functions;
    }

    public InterfaceSymbol GetSymbol() => symbol!;
    
    void IItem.AddSymbol(Compiler compiler) {
        symbol = new InterfaceSymbol(name, this);
        compiler.AddSymbol(name, symbol, position);

        compiler.inInterface = true;
        compiler.EnterNameSpace(symbol.type.space);
        foreach (FunctionItem funItem in functions) {
            funItem.AddSymbol(compiler);
            symbol.conceptFunctions.Add(funItem.name, funItem.GetSymbol());
        }
        compiler.LeaveNameSpace();
        compiler.inInterface = false;
    }

    void IItem.SetSemantic(Compiler compiler) {
        compiler.EnterThisType(symbol!.type);
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }
        compiler.LeaveThisType();
    }
}
