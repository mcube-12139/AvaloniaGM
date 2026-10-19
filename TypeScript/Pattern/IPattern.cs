using TypeScript.Type;

namespace TypeScript.Pattern;

public interface IPattern {
    internal void AddLocal(IType type, bool initialized, Compiler compiler);
    internal void AddParameter(IType type, Compiler compiler);
}
