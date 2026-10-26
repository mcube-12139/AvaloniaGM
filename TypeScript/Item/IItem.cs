namespace TypeScript.Item;

public interface IItem {
    void AddItemSymbol(Compiler compiler);
    void AddImplSymbol(Compiler compiler) {
        // 默认无事发生
    }
    void SetSemantic(Compiler compiler);
}
