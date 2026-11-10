using TypeScript.ExceptionSpace;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ThisParameterSymbol : IValueSymbol {
    readonly IType type;

    internal Intermediate.Parameter interParameter;

    internal ThisParameterSymbol(IType type) {
        this.type = type;

        interParameter = new("this", type.ToIntermediate());
    }

    string ISymbol.GetName() => "this";

    IType IValueSymbol.GetValueType() => type;

    IType IValueSymbol.LazyGetValueType(SourcePosition position, Compiler compiler) => type;

    void IValueSymbol.Store(SourcePosition position, Compiler compiler) {
        throw new SemanticException(SemanticErrorType.NOT_WRITEABLE, ["this"], position);
    }

    void IValueSymbol.EnsureCanLoad(SourcePosition position) {
        // 无事发生
    }

    void IValueSymbol.GenerateLoad(SourcePosition position, StatementContext context) {
        context.AddInstructionWithPosition(new LoadArgumentInstruction(interParameter), position);
    }
}
