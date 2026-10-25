using TypeScript.ExceptionSpace;
using TypeScript.Field;
using TypeScript.Symbol;
using TypeScript.TypeNode;

namespace TypeScript.Item;

public class ClassItem(SourcePosition position, string name, ClassItem.Field[] fields) : IItem {
    public class Field(SourcePosition position, string name, ITypeNode typeNode) {
        public readonly SourcePosition position = position;
        public readonly string name = name;
        public readonly ITypeNode typeNode = typeNode;

        StructField? type;

        public StructField GetFieldType() => type!;

        internal void ResolveType(Compiler compiler) {
            typeNode.SetSemantic(compiler);
            type = new(name, typeNode.GetSelfType());
        }
    }

    public readonly SourcePosition position = position;
    public readonly string name = name;
    public readonly Field[] fields = fields;

    ClassSymbol? symbol;
    StructField[]? classFields;

    public ClassSymbol GetSymbol() => symbol!;
    
    void IItem.AddSymbol(Compiler compiler) {
        symbol = new ClassSymbol(name, this);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) {
        // 无事发生，因为 classFields 是惰性求值的
    }

    internal StructField[] ResolveFields(Compiler compiler) {
        if (classFields == null) {
            List<StructField> classFieldList = [];
            HashSet<string> names = [];
            foreach (Field field in fields) {
                if (!names.Add(field.name)) {
                    throw new SemanticException(SemanticErrorType.FIELD_EXIST, [field.name], field.position);
                }

                field.ResolveType(compiler);
                classFieldList.Add(field.GetFieldType());
            }
            classFields = [.. classFieldList];
        }

        return classFields;
    }
}
