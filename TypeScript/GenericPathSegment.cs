using TypeScript.GenericArgument;
using TypeScript.GenericArgumentNode;
using TypeScript.PathSegment;
using TypeScript.Symbol;

namespace TypeScript;

public class GenericPathSegment {
    public readonly SourcePosition position;
    public readonly IPathSegment segment;
    public readonly IGenericArgumentNode[]? genericArguments;

    internal GenericPathSegment(SourcePosition position, IPathSegment segment, IGenericArgumentNode[]? genericArguments) {
        this.position = position;
        this.segment = segment;
        this.genericArguments = genericArguments;
    }

    ISymbol TryMonomorphize(ISymbol symbol, Compiler compiler) {
        if (genericArguments == null) {
            return symbol;
        }

        IGenericArgument[] arguments = [.. genericArguments.Select(genericArgument => genericArgument.GetArgument(compiler))];
        return symbol.AsGeneric(position).Monomorphize(arguments, position);
    }

    internal ISymbol GetSymbol(Compiler compiler) {
        return TryMonomorphize(segment.GetSymbol(compiler), compiler);
    }

    internal ISymbol GetSymbolInNameSpace(NameSpace space, Compiler compiler) {
        return TryMonomorphize(segment.GetSymbolInNameSpace(space), compiler);
    }
}
