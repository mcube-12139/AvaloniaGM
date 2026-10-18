using TypeScript.Type;

namespace TypeScript.Symbol;

public class VariableSymbol(
    string name,
    IType type
) : IValueSymbol {
    public readonly string name = name;
    public readonly IType type = type;

    /*
ITypeSymbol ISymbol.AsType(TextPosition position, Generator generator) {
throw generator.SemanticError(SemanticErrorType.NOT_TYPE, [name], position);
}
*/
    string ISymbol.GetName() => name;

    IValueSymbol ISymbol.AsValue(TextPosition position, Compiler compiler) {
        return this;
    }

    IType IValueSymbol.LazyGetValueType(TextPosition position, Compiler compiler) => type;

    IType IValueSymbol.GetValueType() => type;

    /*
    void IValueSymbol.Call(TextPosition position, Generator generator) {
        throw new System.NotImplementedException();
    }

    void IValueSymbol.Load(TextPosition position, Generator generator) {
        generator.Load(variable, variableType);
    }

    void IValueSymbol.Store(TextPosition position, Generator generator) {
        generator.Store(variable, variableType);
    }

    void IValueSymbol.LoadIndex(IExpression index, TextPosition position, Generator generator) {
        type.GetIndex(variable, index, position, generator);
    }

    void IValueSymbol.StoreIndex(IExpression right, IExpression index, TextPosition position, Generator generator) {
        type.SetIndex(variable, right, index, position, generator);
    }
    */
}
