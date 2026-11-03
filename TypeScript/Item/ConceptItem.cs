using System.Collections.Immutable;
using TypeScript.GenericParameter;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Item;

public class ConceptItem : IItem {
    public readonly SourcePosition position;
    public readonly string name;
    public readonly IGenericParameter[]? genericParameters;
    public readonly Path[]? supers;
    public readonly TypeConstraint[]? constraints;
    public readonly ImmutableArray<FunctionItem> functions;

    ConceptSymbol? symbol;

    internal ConceptItem(
        SourcePosition position,
        string name,
        IGenericParameter[]? genericParameters,
        Path[]? supers,
        TypeConstraint[]? constraints,
        ImmutableArray<FunctionItem> functions
    ) {
        this.position = position;
        this.name = name;
        this.genericParameters = genericParameters;
        this.supers = supers;
        this.constraints = constraints;
        this.functions = functions;
    }

    public ConceptSymbol GetSymbol() => symbol!;
    
    void IItem.AddItemSymbol(Compiler compiler) {
        symbol = new ConceptSymbol(name, compiler.GetSpaceState(), this);
        compiler.AddSymbol(name, symbol, position);

        compiler.conceptContext = ConceptContext.CONCEPT;
        foreach (FunctionItem funItem in functions) {
            symbol.conceptFunctions.Add(funItem.name, funItem.CreateConceptFunction(compiler));
        }
        compiler.conceptContext = ConceptContext.NONE;
    }

    void IItem.SetSemantic(Compiler compiler) {
        if (!symbol!.resolved) {
            Resolve(compiler);
        }

        compiler.thisTypeSymbol = symbol.thisTypeSymbol;
        foreach (FunctionItem funItem in functions) {
            funItem.SetSemantic(compiler);
        }
        compiler.thisTypeSymbol = null;
    }

    internal void Resolve(Compiler compiler) {
        symbol!.resolved = true;
        var unresolved = GenericType.Unresolved([symbol!]);
        symbol.thisTypeSymbol = new(unresolved);

        compiler.thisTypeSymbol = symbol.thisTypeSymbol;
        foreach (FunctionItem funItem in functions) {
            funItem.ResolveType(compiler);
        }
        compiler.thisTypeSymbol = null;

        unresolved.ResolveBounds(position, compiler);
    }
}
