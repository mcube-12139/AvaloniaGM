using TypeScript.GenericArgument;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class TypeParameterSymbol : ITypeSymbol, IGenericParameterSymbol {
    public readonly string name;
    public readonly GenericType type;

    internal TypeParameterSymbol(string name, GenericType type) {
        this.name = name;
        this.type = type;
    }

    string ISymbol.GetName() => name;

    IType ITypeSymbol.GetSelfType() => type;

    IType ITypeSymbol.LazyGetSelfType(SourcePosition position, Compiler compiler) => type;

    void IGenericParameterSymbol.SetArgument(IGenericArgument argument, SourcePosition position) {
        type.concreteType = argument.AsType(position);
    }

    void IGenericParameterSymbol.SetArgumentOrThrow(IGenericArgument argument) {
        type.concreteType = (IType)argument;
    }
}
