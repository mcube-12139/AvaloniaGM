using TypeScript.Expression;
using TypeScript.GenericArgument;
using TypeScript.GenericArgumentNode;
using TypeScript.PathSegment;
using TypeScript.Symbol;
using TypeScript.Type;
using TypeScript.TypeNode;

namespace TypeScript;

public class Path : IPlaceExpression, ITypeNode {
    public class Segment {
        public readonly SourcePosition position;
        public readonly IPathSegment segment;
        public readonly IGenericArgumentNode[]? genericArguments;

        internal Segment(SourcePosition position, IPathSegment segment, IGenericArgumentNode[]? genericArguments) {
            this.position = position;
            this.segment = segment;
            this.genericArguments = genericArguments;
        }

        ISymbol TryMonomorphize(ISymbol symbol, Compiler compiler) {
            if (genericArguments == null) {
                return symbol;
            }

            foreach (IGenericArgumentNode argument in genericArguments) {
                argument.SetSemantic(compiler);
            }

            IGenericArgument[] arguments = [.. genericArguments.Select(genericArgument => genericArgument.GetArgument(compiler))];
            return symbol.AsGeneric(position).Monomorphize(arguments, position, compiler);
        }

        internal ISymbol GetSymbol(Compiler compiler) {
            return TryMonomorphize(segment.GetSymbol(compiler), compiler);
        }

        internal ISymbol GetSymbolInNameSpace(NameSpace space, Compiler compiler) {
            return TryMonomorphize(segment.GetSymbolInNameSpace(space), compiler);
        }
    }

    public readonly SourcePosition position;
    public readonly Segment[] segments;
    public readonly bool fromTop;

    internal Path(SourcePosition position, Segment[] segments, bool fromTop) {
        this.position = position;
        this.segments = segments;
        this.fromTop = fromTop;
    }

    ISymbol? symbol;
    IType? type;

    SourcePosition IExpression.GetPosition() => position;

    IType ITypeNode.GetSelfType(Compiler compiler) => GetSelfType(compiler);

    void IGenericArgumentNode.SetSemantic(Compiler compiler) {
        symbol = FindSymbol(compiler);
    }

    FunctionType IExpression.SetCallSemantic(IType? target, IExpression[] parameters, Compiler compiler) {
        ISymbol maybeGenericSymbol = FindSymbol(compiler);
        IValueSymbol valueSymbol = maybeGenericSymbol.GetGenericOmittedCallSymbol(type, parameters, position, compiler);
        symbol = valueSymbol;
        FunctionType funType = valueSymbol.LazyGetValueType(position, compiler).AsFunctionType(position, compiler);
        type = funType;

        return funType;
    }

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        symbol = FindSymbol(compiler);
        IValueSymbol valueSymbol = symbol!.AsValue(position);
        type = valueSymbol.LazyGetValueType(position, compiler);
        valueSymbol.EnsureCanLoad(position);
    }

    ISymbol FindSymbol(Compiler compiler) {
        ISymbol nowSymbol;
        if (!fromTop) {
            nowSymbol = segments[0].GetSymbol(compiler);
        } else {
            nowSymbol = segments[0].GetSymbolInNameSpace(compiler.GetTopNameSpace(), compiler);
        }

        for (int i = 1; i != segments.Length; ++i) {
            nowSymbol = segments[i].GetSymbolInNameSpace(nowSymbol.GetNameSpace(position), compiler);
        }

        return nowSymbol;
    }

    internal IType GetSelfType(Compiler compiler) {
        return symbol!.AsType(position).LazyGetSelfType(position, compiler);
    }

    IType IExpression.GetResultType() => type!;

    public ISymbol GetSymbol() => symbol!;

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        symbol = FindSymbol(compiler);
        IValueSymbol valueSymbol = symbol!.AsValue(position);
        type = valueSymbol.LazyGetValueType(position, compiler);
        valueSymbol.Store(position, compiler);
    }

    IGenericArgument IGenericArgumentNode.GetArgument(Compiler compiler) => GetSelfType(compiler);
}
