namespace TypeScript.Intermediate.FunctionBody;

public class HostBody: IFunctionBody {
    public readonly string hostName;

    internal HostBody(string hostName) {
        this.hostName = hostName;
    }
}
