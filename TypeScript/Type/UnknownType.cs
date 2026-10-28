using TypeScript.GenericArgument;

namespace TypeScript.Type;

public class UnknownType : IType {
    string IGenericArgument.GetAppearance() => "unknown";
}
