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

    internal readonly ExpressionHead head;
    public readonly Segment[] segments;
    public readonly bool fromTop;

    internal Path(SourcePosition position, Segment[] segments, bool fromTop) {
        head = new(position);
        this.segments = segments;
        this.fromTop = fromTop;
    }

    ISymbol? symbol;
    IType? type;

    ExpressionHead IExpression.GetHead() => head;

    IType ITypeNode.GetSelfType(Compiler compiler) => GetSelfType(compiler);

    void IGenericArgumentNode.SetSemantic(Compiler compiler) {
        symbol = FindSymbol(compiler);
    }

    FunctionType IExpression.SetCallSemantic(IType? target, IExpression[] parameters, Compiler compiler) {
        ISymbol maybeGenericSymbol = FindSymbol(compiler);
        IValueSymbol valueSymbol = maybeGenericSymbol.GetGenericOmittedCallSymbol(type, parameters, head.position, compiler);
        symbol = valueSymbol;
        FunctionType funType = valueSymbol.LazyGetValueType(head.position, compiler).AsFunctionType(head.position, compiler);
        type = funType;

        return funType;
    }

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        symbol = FindSymbol(compiler);
        IValueSymbol valueSymbol = symbol!.AsValue(head.position);
        type = valueSymbol.LazyGetValueType(head.position, compiler);
        valueSymbol.EnsureCanLoad(head.position);
    }

    ISymbol FindSymbol(Compiler compiler) {
        ISymbol nowSymbol;
        if (!fromTop) {
            nowSymbol = segments[0].GetSymbol(compiler);
        } else {
            nowSymbol = segments[0].GetSymbolInNameSpace(compiler.GetTopNameSpace(), compiler);
        }

        for (int i = 1; i != segments.Length; ++i) {
            nowSymbol = segments[i].GetSymbolInNameSpace(nowSymbol.GetNameSpace(head.position), compiler);
        }

        return nowSymbol;
    }

    internal IType GetSelfType(Compiler compiler) {
        return symbol!.AsType(head.position).LazyGetSelfType(head.position, compiler);
    }

    IType IExpression.GetResultType() => type!;

    public ISymbol GetSymbol() => symbol!;

    void IPlaceExpression.SetAssignSemantic(Compiler compiler) {
        symbol = FindSymbol(compiler);
        IValueSymbol valueSymbol = symbol!.AsValue(head.position);
        type = valueSymbol.LazyGetValueType(head.position, compiler);
        valueSymbol.Store(head.position, compiler);
    }

    IGenericArgument IGenericArgumentNode.GetArgument(Compiler compiler) => GetSelfType(compiler);
}
