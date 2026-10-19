namespace TypeScript.Type;

public class ClassField(string name, IType type) {
    public readonly string name = name;
    public readonly IType type = type;
}

public class ClassMethod(string name, FunctionType type) {
    public readonly string name = name;
    public readonly FunctionType type = type;
}

public class ClassType(string name, ClassField[] fields, ClassMethod[] methods) : IType {
    public readonly string name = name;
    public readonly ClassField[] fields = fields;
    public readonly ClassMethod[] methods = methods;

    public bool EqualType(IType other) {
        return other == this;
    }

    public string GetAppearance() => name;
}
