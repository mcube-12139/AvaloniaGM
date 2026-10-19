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

    public FunctionSymbol GetSymbol() => symbol!;
    
    void IItem.AddSymbol(Compiler compiler) {
        symbol = FunctionSymbol.NewUnresolved(name, this, null);
        compiler.AddSymbol(name, symbol, position);
    }

    void IItem.SetSemantic(Compiler compiler) {
        resultNode?.SetSemantic(compiler);

        IType resultType = resultNode?.GetSelfType() ?? TupleType.EMPTY;

        compiler.EnterFunction(resultType);

        foreach (IParameter parameter in parameters) {
            parameter.SetSemantic(compiler);
        }
        body.SetSemantic(compiler);

        if (resultType != TupleType.EMPTY && !compiler.GetReturned()) {
            // 返回类型非()必须显式返回
            throw compiler.SemanticError(ExceptionSpace.SemanticErrorType.MAY_NOT_RETURN, [name], position);
        }

        compiler.LeaveFunction();

        symbol!.type = new FunctionType([.. parameters.Select(parameter => parameter.GetParameterType())], resultType);
    }

    internal FunctionType ResolveType(Compiler compiler) {
        IType[] parameterTypes = [.. parameters.Select(parameter => parameter.LazyGetParameterType(compiler))];
        IType resultType = resultNode?.LazyGetSelfType(compiler) ?? TupleType.EMPTY;
        return new(parameterTypes, resultType);
    }
}
