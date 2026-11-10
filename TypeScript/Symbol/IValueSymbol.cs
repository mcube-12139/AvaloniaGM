using TypeScript.Expression;
using TypeScript.GenerateContext;
using TypeScript.Intermediate.Instruction;
using TypeScript.Type;

namespace TypeScript.Symbol;

public interface IValueSymbol: ISymbol {
    IValueSymbol ISymbol.AsValue(SourcePosition position) {
        return this;
    }
    IType GetValueType();
    internal IType LazyGetValueType(SourcePosition position, Compiler compiler);
    internal void Store(SourcePosition position, Compiler compiler);
    internal void EnsureCanLoad(SourcePosition position);
    internal void GenerateStore(SourcePosition position, StatementContext context) => throw new NotImplementedException();
    internal void GenerateLoad(SourcePosition position, StatementContext context) => throw new NotImplementedException();
    internal void GenerateCall(CallExpression call, SourcePosition position, StatementContext context) {
        GenerateLoad(position, context);
        call.GenerateParameters(context);
        context.AddInstruction(new CallPointerInstruction(GetValueType().AsFunctionTypeOrThrow().intermediateType));
    }
}
