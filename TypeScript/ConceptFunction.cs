using TypeScript.ExceptionSpace;
using TypeScript.Item;
using TypeScript.NameSpace;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript;

internal class ConceptFunction(string name, bool method) {
    internal readonly string name = name;
    internal readonly bool method = method;

    internal IType[]? parameterTypes;
    internal IType? resultType;

    internal void EnsureSatisfy(FunctionSymbol fun, SourcePosition position, Compiler compiler) {
        if (fun.method != method) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_METHOD, [name], position);
        }

        FunctionType funType = fun.LazyGetValueType(position, compiler);
        if (funType.resultType != resultType!.TryGetConcreteType()) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_TYPE, [funType.resultType.GetAppearance(), resultType.GetAppearance()], position);
        }

        if (funType.parameterTypes.Length != parameterTypes!.Length) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_PARAMETER_COUNT, [funType.parameterTypes.Length.ToString(), parameterTypes!.Length.ToString()], position);
        }

        for (int i = 0; i != parameterTypes.Length; ++i) {
            IType implType = funType.parameterTypes[i];
            IType definedType = parameterTypes[i].TryGetConcreteType();
            if (implType != definedType) {
                throw new SemanticException(SemanticErrorType.NOT_SATISFY_TYPE, [implType.GetAppearance(), definedType.GetAppearance()], position);
            }
        }
    }
}
