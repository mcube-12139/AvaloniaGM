using TypeScript.Symbol;
using TypeScript.TypeNode;

namespace TypeScript.Item;

public class ClassFieldNode(SourcePosition position, string name, ITypeNode type) {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly ITypeNode type = type;
}

public class ClassItem(SourcePosition position, string name, ClassFieldNode[] fields) : IItem {
    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly ClassFieldNode[] fields = fields;

    ClassSymbol? symbol;

    public ClassSymbol GetSymbol() => symbol!;
    
    void IItem.AddSymbol(Compiler compiler) {
        symbol = new ClassSymbol(name, this);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) {
    }
}
