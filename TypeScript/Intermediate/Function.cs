namespace TypeScript.Intermediate;

public class Function {
    public readonly string name;
    public readonly IFunctionBody body;

    internal Function(string name, IFunctionBody body) {
        this.name = name;
        this.body = body;
    }
}
