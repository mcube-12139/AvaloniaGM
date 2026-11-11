using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.GenerateContext;
using TypeScript.Intermediate;
using TypeScript.Intermediate.Instruction;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class ConceptFunctionSymbol : ISymbol {
    public readonly string name;
    internal readonly NameSpace itemSpace;
    public readonly bool method;
    public readonly uint index;
    internal FunctionType? type;
    readonly FunctionItem? item;

    readonly List<CallVirtualInstruction> callPromises = [];
    internal VirtualFunction? virtualFun;

    internal ConceptFunctionSymbol(string name, NameSpace itemSpace, bool method, uint index, FunctionItem? item) {
        this.name = name;
        this.itemSpace = itemSpace;
        this.method = method;
        this.index = index;
        this.item = item;
    }

    string ISymbol.GetName() => name;

    internal void LazyGetValueType(Compiler compiler) {
        // 不能调用 SetSemantic，会无限递归
        if (type == null) {
            compiler.PushNameSpace(itemSpace);
            type = item!.ResolveType(compiler);
            compiler.PopNameSpace();
        }
    }

    IType ISymbol.GetMethodCallResultType(IExpression[] parameters, SourcePosition position, Compiler compiler) {
        if (!method) {
            throw new SemanticException(SemanticErrorType.NOT_METHOD, [name], position);
        }

        LazyGetValueType(compiler);
        type!.EnsureCanCallMethod(parameters, position, compiler);

        return type!.resultType!;
    }

    internal void EnsureSatisfySelf(FunctionSymbol funSymbol, SourcePosition position, Compiler compiler) {
        if (funSymbol.method != method) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
        }

        FunctionType funType = funSymbol.LazyGetValueType(position, compiler);
        LazyGetValueType(compiler);

        if (type!.resultType != funType.resultType) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
        }

        if (type!.parameterTypes.Length != funType.parameterTypes.Length) {
            throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
        }

        for (int i = 1; i < type!.parameterTypes.Length; ++i) {
            if (funType.parameterTypes[i] != type!.parameterTypes[i]) {
                throw new SemanticException(SemanticErrorType.NOT_SATISFY_CONCEPT_FUNCTION, [name], position);
            }
        }
    }

    internal void ResolveVirtualFunction(VirtualFunction fun) {
        virtualFun = fun;
        foreach (CallVirtualInstruction instruction in callPromises) {
            instruction.fun = fun;
        }
    }

    void ISymbol.GenerateCallMethod(SourcePosition position, StatementContext context) {
        CallVirtualInstruction instruction = new(virtualFun);
        if (virtualFun == null) {
            callPromises.Add(instruction);
        }
        context.AddInstructionWithPosition(instruction, position);
    }
}
