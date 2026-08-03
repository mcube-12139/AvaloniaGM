using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Statements {
    internal interface IStatement {
        void Execute(TypeScriptGenerator generator);
    }
}
