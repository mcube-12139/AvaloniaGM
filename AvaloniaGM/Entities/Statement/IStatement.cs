using AvaloniaGM.Services;

namespace AvaloniaGM.Entities.Statement {
    internal interface IStatement {
        void Execute(CodeGenerator generator);
    }
}
