using AvaloniaGM.TypeScript;

namespace AvaloniaGM.TypeScript.Statements {
    internal interface IStatement {
        void Execute(Generator generator);
    }
}
