using TypeScript.Type;

namespace TypeScript.Pattern;

public interface IPattern {
    /*
    void AddParameter(ITypeNode typeNode, Generator generator);
    void AddVariable(ITypeNode? typeNode, IExpression? initializer, Generator generator, TextPosition position);
    */
    internal void AddVariable(IType type, Compiler compiler);
    internal void AddParameter(IType type, Compiler compiler);
}
