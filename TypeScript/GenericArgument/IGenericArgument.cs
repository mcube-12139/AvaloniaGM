using TypeScript.Type;

namespace TypeScript.GenericArgument;

public interface IGenericArgument {
    internal string GetAppearance();
    internal IType AsType(SourcePosition position);
}
