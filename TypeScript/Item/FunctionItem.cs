using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;

namespace TypeScript.Item;

internal class FunctionItem(TextPosition position, string name, IParameter[] parameters, ITypeNode? resultNode, BlockStatement body) : IItem {
    FunctionSymbol? symbol;
    
    void IItem.AddSymbol(Compiler compiler) {
        /*
        (UndertaleFunction fun, code) = generator.CreateFunction(name);
        symbol = FunctionSymbol.NewUnresolved(name, fun, this);
        generator.AddSymbol(name, symbol, position);
        */
        symbol = FunctionSymbol.NewUnresolved(name);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) {
        IType resultType = resultNode?.GetSharkType(compiler) ?? TupleType.EMPTY;

        compiler.EnterFunction(resultType);

        foreach (IParameter parameter in parameters) {
            parameter.SetSemantic(compiler);
        }
        resultNode?.SetSemantic(compiler);
        body.SetSemantic(compiler);

        compiler.LeaveFunction();

        symbol!.type = new FunctionType([.. parameters.Select(parameter => parameter.GetSharkType(compiler))], resultType);
    }

    /*
    internal FunctionType ResolveType(Generator generator) {
        IType[] parameterTypes = [.. parameters.Select(parameter => parameter.GetSharkType(generator))];
        GetResultType(generator);
        return new(parameterTypes, resultType!);
    }
    */
}
