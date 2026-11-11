using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class InterfaceType : IType {
    public readonly string name;

    internal readonly NameSpace space;

    internal Intermediate.Type.InterfaceType? intermediateType;

    public ConceptFunctionSymbol GetConceptFunction(string name) => (ConceptFunctionSymbol)space.GetSymbolOrException(name);

    internal InterfaceType(string name) {
        this.name = name;

        space = new(null);
    }

    bool IType.EqualType(IType other) {
        return other == this;
    }

    string IType.GetAppearance() => name;

    Intermediate.Type.IType IType.ToIntermediate() => intermediateType!;

    ISymbol IType.GetAssociatedSymbol(string name, SourcePosition position) {
        return space.GetSymbol(name, position);
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        right.SetTargetType(this);
    }
}
