using TypeScript.ExceptionSpace;
using TypeScript.GenericArgument;
using TypeScript.NameSpace;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class TypeParameterSymbol : ITypeSymbol, IGenericParameterSymbol {
    public readonly string name;
    public readonly GenericType type;

    string ISymbol.GetName() => name;
    IType ITypeSymbol.GetSelfType() => type;
    SymbolSpace ISymbol.GetNameSpace(SourcePosition position) => ((IType)type).GetNameSpace();

    internal TypeParameterSymbol(string name, GenericType type) {
        this.name = name;
        this.type = type;
    }

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;

    void IGenericParameterSymbol.SetArgument(IGenericArgument argument, SourcePosition position) {
        type.concreteType = argument.AsType(position);
    }

    void IGenericParameterSymbol.SetArgumentOrThrow(IGenericArgument argument) {
        type.concreteType = (IType)argument;
    }

    IGenericArgument IGenericParameterSymbol.SetInferredArgument(GenericInferContext context) {
        IType inferredType = context.TryGetInferredType(type) ?? throw new SemanticException(SemanticErrorType.GENERIC_PARAMETER_NOT_INFERRED, [name], context.position);

        type.concreteType = inferredType;
        return inferredType;
    }
}
