using TypeScript.GenerateContext;
using TypeScript.Type;

namespace TypeScript.Field;

public interface IField {
    IType GetFieldType();
    internal void EnsureCanSet(SourcePosition position);
    internal void GenerateStore(StatementContext context) { }
    internal void GenerateLoad(StatementContext context) { }
}
