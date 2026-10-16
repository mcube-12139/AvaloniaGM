using TypeScript.Type;

namespace TypeScript.Pattern;

public interface IPattern {
    /*
    void AddParameter(ITypeNode typeNode, Generator generator);
    void AddVariable(ITypeNode? typeNode, IExpression? initializer, Generator generator, TextPosition position);
    */
    internal void AddSymbol(IType type, Compiler compiler);
}
