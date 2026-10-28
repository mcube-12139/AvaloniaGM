using TypeScript.ExceptionSpace;
using TypeScript.GenericArgument;
using TypeScript.Item;
using TypeScript.Type;

namespace TypeScript.Symbol;

public class GenericFunctionSymbol : IGenericSymbol {
    public readonly Dictionary<IGenericArgument[], FunctionSymbol> monomorphized = [];

    public readonly string name;
    internal readonly NameSpace itemSpace;
    public readonly bool method;
    readonly FunctionItem? item;

    internal IGenericParameterSymbol[]? parameterSymbols;
    internal FunctionType? type;

    internal GenericFunctionSymbol(string name, NameSpace itemSpace, bool method, FunctionItem? item) {
        this.name = name;
        this.itemSpace = itemSpace;
        this.method = method;
        this.item = item;
    }

    string ISymbol.GetName() => name;

    public IEnumerable<FunctionSymbol> IterateSymbolSetGenericArguments() {
        foreach ((IGenericArgument[] arguments, FunctionSymbol funSymbol) in monomorphized) {
            for (int i = 0; i != arguments.Length; ++i) {
                parameterSymbols![i].SetArgumentOrThrow(arguments[i]);
            }
            yield return funSymbol;
        }
    }

    ISymbol IGenericSymbol.Monomorphize(IGenericArgument[] arguments, SourcePosition position, Compiler compiler) {
        if (type == null) {
            compiler.PushNameSpace(itemSpace);
            item!.ResolveType(compiler);
            compiler.PopNameSpace();
        }

        if (!monomorphized.TryGetValue(arguments, out FunctionSymbol? result)) {
            if (arguments.Length != parameterSymbols!.Length) {
                throw new SemanticException(SemanticErrorType.WRONG_PARAMETER_COUNT, [arguments.Length.ToString(), parameterSymbols.Length.ToString()], position);
            }

            for (int i = 0; i != arguments.Length; ++i) {
                parameterSymbols[i].SetArgument(arguments[i], position);
            }

            FunctionType monomorphizedType = type!.Monomorphize();
            result = new(
                $"{name}_{string.Join('_', arguments.Select(argument => argument.GetAppearance()))}",
                monomorphizedType,
                itemSpace,
                method,
                null,
                null
            );
            monomorphized[arguments] = result;
        }

        return result;
    }
}
