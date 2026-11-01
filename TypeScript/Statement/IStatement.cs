using TypeScript.Item;

namespace TypeScript.Statement;

public interface IStatement: IItem {
    SourcePosition GetPosition();
}
