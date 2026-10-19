using TypeScript.Type;

namespace TypeScript;

internal class FunctionContext(
    IType resultType,
    bool returned
) {
    internal readonly IType resultType = resultType;
    internal readonly bool returned = returned;
}
