using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Symbol;

namespace TypeScript.Type;

public class InterfaceType : IType {
    public readonly string name;

    internal readonly NameSpace space;

    public ConceptFunctionSymbol GetConceptFunction(string name) => (ConceptFunctionSymbol)space.GetSymbolOrException(name);

    internal InterfaceType(string name) {
        this.name = name;

        space = new(null);
    }

    bool IType.EqualType(IType other) {
        return other == this;
    }

    string IType.GetAppearance() => name;

    ISymbol IType.GetAssociatedSymbol(string name, SourcePosition position) {
        return space.GetSymbol(name, position);
    }

    void IType.EnsureCanAssign(IExpression right, SourcePosition position, Compiler compiler) {
        right.SetSemantic(this, compiler);
        IType rightType = right.GetResultType();
        if (!TypeHelp.IsType(rightType, this)) {
            throw new SemanticException(SemanticErrorType.OPERATION_NOT_EXIST, [$"{name} = {rightType.GetAppearance()}"], position);
        }
    }
}
