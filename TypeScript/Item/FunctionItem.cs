using TypeScript.Parameter;
using TypeScript.Statement;
using TypeScript.Symbol;
using TypeScript.TypeNode;
using TypeScript.Type;

namespace TypeScript.Item;

public class FunctionItem(TextPosition position, string name, IParameter[] parameters, ITypeNode? resultNode, BlockStatement body) : IItem {
    public readonly TextPosition position = position;
    public readonly string name = name;
    public readonly IParameter[] parameters = parameters;
    public readonly ITypeNode? resultNode = resultNode;
    public readonly BlockStatement body = body;

    FunctionSymbol? symbol;
    
    void IItem.AddSymbol(Compiler compiler) {
        /*
        (UndertaleFunction fun, code) = generator.CreateFunction(name);
        symbol = FunctionSymbol.NewUnresolved(name, fun, this);
        generator.AddSymbol(name, symbol, position);
        */
        symbol = FunctionSymbol.NewUnresolved(name, null);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) {
        foreach (IParameter parameter in parameters) {
            parameter.SetSemantic(compiler);
        }
        resultNode?.SetSemantic(compiler);

        IType resultType = resultNode?.GetSharkType() ?? TupleType.EMPTY;

        compiler.EnterFunction(resultType);

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
