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
        symbol = new ConceptSymbol(name, this);
        compiler.AddSymbol(name, symbol, position);

        compiler.conceptContext = ConceptContext.CONCEPT;
        foreach (FunctionItem funItem in functions) {
            symbol.conceptFunctions.Add(funItem.name, (GenericFunctionSymbol)funItem.CreateSymbol(compiler));
        }
        compiler.conceptContext = ConceptContext.NONE;
    }

    void IItem.SetSemantic(Compiler compiler) {
        compiler.thisTypeSymbol = new(new GenericType([symbol!], null));
        foreach (FunctionItem funItem in functions) {
            funItem.SetConceptSemantic(compiler);
        }
        compiler.thisTypeSymbol = null;
    }
}
