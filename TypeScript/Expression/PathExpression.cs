using TypeScript.Symbol;
using TypeScript.Type;

namespace TypeScript.Expression;

public class PathExpression(TextPosition position, string[] segments) : IPlaceExpression {
    public readonly TextPosition position = position;
    public readonly string[] segments = segments;

    IType? type;
    IValueSymbol? symbol;

    TextPosition IExpression.GetPosition() => position;

    void IExpression.SetSemantic(IType? target, Compiler compiler) {
        symbol = compiler.GetSymbol(segments[0], position).AsValue(position, compiler);
        type = symbol.LazyGetValueType(position, compiler);
    }

    IType IExpression.GetResultType() => type!;

    public IValueSymbol GetSymbol() => symbol!;

    /*
    void IPlaceExpression.Assign(IExpression right, Generator generator) {
        right.Evaluate(generator);

        GetResultType(generator);
        type!.Assign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.AddAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.AddAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.SubtractAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.SubtractAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.MultiplyAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.MultiplyAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.DivideAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.DivideAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.ModuloAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.ModuloAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.BitAndAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.BitAndAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.BitOrAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.BitOrAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.BitXorAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.BitXorAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.LeftShiftAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.LeftShiftAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void IPlaceExpression.RightShiftAssign(IExpression right, Generator generator) {
        Evaluate(generator);
        GetResultType(generator);
        type!.RightShiftAssign(right, position, generator);

        SetSymbol(generator);
        symbol!.AsValue(position, generator).Store(position, generator);
    }

    void SetSymbol(Generator generator) {
        symbol ??= generator.GetSymbol(segments[0], position);
    }

    void IExpression.Call(Generator generator) {
        SetSymbol(generator);
        IValueSymbol valueSymbol = symbol!.AsValue(position, generator);
        valueSymbol.Call(position, generator);
    }

    void IExpression.GetIndex(IExpression index, Generator generator) {
        SetSymbol(generator);
        symbol!.AsValue(position, generator).LoadIndex(index, position, generator);
    }

    void IExpression.SetIndex(IExpression right, IExpression index, Generator generator) {
        SetSymbol(generator);
        symbol!.AsValue(position, generator).StoreIndex(right, index, position, generator);
    }

    UndertaleVariable IExpression.GetIndexDuplicate(IExpression index, Generator generator) {
        throw new System.NotImplementedException();
    }
    */
}
