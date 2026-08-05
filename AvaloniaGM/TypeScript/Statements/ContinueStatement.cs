using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Statements {
    internal class ContinueStatement(TextPosition position, string? label): IStatement {
        void IStatement.Execute(Generator generator) {
            ILoopStatement loop = generator.GetLoop(label, position);
            uint startOffset = loop.GetStart();
            uint branchStart = generator.GetByteCount();
            UndertaleInstruction branch = generator.Branch();
            branch.JumpOffset = (int)(startOffset - branchStart) / 4;
        }
    }
}
