using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Pattern;

public class IdentifierPattern(TextPosition position, string name) : IPattern {
    public readonly TextPosition position = position;
    public readonly string name = name;
    ISymbol? symbol;

    public ISymbol GetSymbol() => symbol!;

    void IPattern.AddVariable(IType type, bool initialized, Compiler compiler) {
        symbol = new VariableSymbol(name, type, initialized);
        compiler.AddSymbol(name, symbol, position);
    }

    void IPattern.AddParameter(IType type, Compiler compiler) {
        symbol = new ParameterSymbol(name, type);
        compiler.AddSymbol(name, symbol, position);
    }

    /*
    void IPattern.AddParameter(ITypeNode typeNode, Generator generator) {
        uint index = generator.NextParameterIndex();
        UndertaleVariable variable = generator.AddSelfVariable($"argument{index}", true);
        IType type = typeNode.GetSharkType(generator);
        generator.AddSymbol(name, new VariableSymbol(name, UndertaleInstruction.VariableType.Normal, variable, type), position);
    }

    void IPattern.AddVariable(ITypeNode? typeNode, IExpression? initializer, Generator generator, TextPosition position) {
        UndertaleVariable variable = generator.AddLocalVariable(name);

        IType type;
        if (initializer != null) {
            initializer.Evaluate(generator);
            type = initializer.GetResultType(generator);
            generator.Store(variable, UndertaleInstruction.VariableType.Normal);
        } else {
            throw new System.NotImplementedException();
        }

        generator.AddSymbol(name, new VariableSymbol(name, UndertaleInstruction.VariableType.Normal, variable, type), position);
    }
    */
}
