using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IConceptSymbol: ISymbol {
    IConceptSymbol ISymbol.AsConcept(SourcePosition position) => this;
    internal void SetThisType(IType type, Compiler compiler) {
        throw new NotImplementedException();
    }
    internal Dictionary<string, ConceptFunction> GetFunctions(Compiler compiler);
}
