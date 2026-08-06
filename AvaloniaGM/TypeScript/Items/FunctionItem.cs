using AvaloniaGM.TypeScript.Parameters;
using AvaloniaGM.TypeScript.Statements;
using AvaloniaGM.TypeScript.Symbols;
using AvaloniaGM.TypeScript.TypeNodes;
using AvaloniaGM.TypeScript.Types;
using System.Linq;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Items {
    internal class FunctionItem(TextPosition position, string name, IParameter[] parameters, ITypeNode? resultNode, BlockStatement body) : IItem {
        FunctionSymbol? symbol;
        UndertaleCode? code;
        IType? resultType;
        
        void IItem.AddSymbol(Generator generator) {
            (UndertaleFunction fun, code) = generator.CreateFunction(name);
            symbol = FunctionSymbol.NewUnresolved(name, fun, this);
            generator.AddSymbol(name, symbol, position);
        }

        void IItem.Generate(Generator generator) {
            generator.EnterFunction(code!, GetResultType(generator));

            foreach (IParameter parameter in parameters) {
                parameter.AddVariable(generator);
            }

            body.Execute(generator);

            generator.LeaveFunction(code!);
        }

        IType GetResultType(Generator generator) {
            resultType ??= resultNode?.GetSharkType(generator) ?? TupleType.EMPTY;
            return resultType;
        }

        internal FunctionType ResolveType(Generator generator) {
            IType[] parameterTypes = [.. parameters.Select(parameter => parameter.GetSharkType(generator))];
            GetResultType(generator);
            return new(parameterTypes, resultType!);
        }
    }
}
