using TypeScript.ExceptionSpace;
using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Parameter;

public class ThisParameter(SourcePosition position) {
    public readonly SourcePosition position = position;

    ThisParameterSymbol? symbol;
    IType? type;

    public IType GetParameterType() => type!;
    public ThisParameterSymbol GetSymbol() => symbol!;

    internal void SetSemantic(Compiler compiler) {
        // this 只能用于接口或实现内的函数
        type = compiler.GetThisType() ?? throw new SemanticException(SemanticErrorType.INVALID_THIS, [], position);
    }

    internal void AddSymbol(Compiler compiler) {
        symbol = new(type!);
        compiler.thisParameterSymbol = symbol;
    }
}
