namespace TypeScript.Symbol;

public interface IConceptSymbol: ISymbol {
    IConceptSymbol ISymbol.AsConcept(SourcePosition position) => this;
}
