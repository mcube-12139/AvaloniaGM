using TypeScript.Type;

namespace TypeScript;

internal class GenericInferContext(SourcePosition position, Compiler compiler) {
    internal readonly SourcePosition position = position;
    internal readonly Compiler compiler = compiler;

    readonly Dictionary<GenericType, IType> inferredTypes = [];

    internal void AddInferedType(GenericType genericType, IType inferredType) {
        inferredTypes.Add(genericType, inferredType);
    }

    internal IType? TryGetInferredType(GenericType genericType) {
        return inferredTypes.GetValueOrDefault(genericType);
    }
}
