namespace TypeScript.Item;

public interface IItem {
    void AddSymbol(Compiler compiler);
    void SetSemantic(Compiler compiler);
}
