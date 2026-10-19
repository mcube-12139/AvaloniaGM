using TypeScript.Type;

namespace TypeScript.Pattern;

public interface IPattern {
    internal void AddVariable(IType type, bool initialized, Compiler compiler);
    internal void AddParameter(IType type, Compiler compiler);
}
