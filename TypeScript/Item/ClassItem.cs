using TypeScript.Symbol;
using TypeScript.TypeNode;

namespace TypeScript.Item;

public class ClassField(SourcePosition position, string name, ITypeNode type) {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly ITypeNode type = type;
}

public class ClassItem(SourcePosition position, string name, ClassField[] fields, FunctionItem[] functions) : IItem {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly ClassField[] fields = fields;
    public readonly FunctionItem[] functions = functions;

    ClassSymbol? symbol;

    public ClassSymbol GetSymbol() => symbol!;
    
    void IItem.AddSymbol(Compiler compiler) {
        symbol = new ClassSymbol(name, this);
        compiler.AddSymbol(name, symbol, position);

        compiler.EnterNameSpace(symbol.space);
        foreach (IItem funItem in functions) {
            funItem.AddSymbol(compiler);
        }
        compiler.LeaveNameSpace();
    }

    void IItem.SetSemantic(Compiler compiler) {
        foreach (IItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }
    }
}
