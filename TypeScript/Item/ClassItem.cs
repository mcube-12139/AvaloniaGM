using TypeScript.Symbol;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript.Item;

public class ClassItem(SourcePosition position, string name, ClassItem.Field[] fields) : IItem {
    public class Field(SourcePosition position, string name, ITypeNode typeNode) {
        public readonly SourcePosition position = position;
        public readonly string name = name;
        public readonly ITypeNode typeNode = typeNode;

        ClassType.Field? type;

        public ClassType.Field GetFieldType() => type!;

        internal void ResolveType(Compiler compiler) {
            typeNode.SetSemantic(compiler);
            type = new(name, typeNode.GetSelfType());
        }
    }

    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly Field[] fields = fields;

    ClassSymbol? symbol;
    ClassType.Field[]? classFields;

    public ClassSymbol GetSymbol() => symbol!;
    
    void IItem.AddSymbol(Compiler compiler) {
        symbol = new ClassSymbol(name, this);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) {
    }

    internal ClassType.Field[] ResolveFields(Compiler compiler) {
        if (classFields == null) {
            List<ClassType.Field> classFieldList = [];
            foreach (Field field in fields) {
                field.ResolveType(compiler);
                classFieldList.Add(field.GetFieldType());
            }
            classFields = [.. classFieldList];
        }

        return classFields;
    }
}
