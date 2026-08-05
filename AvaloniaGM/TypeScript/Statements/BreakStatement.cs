using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Statements {
    internal class BreakStatement(TextPosition position, string? label): IStatement {
        void IStatement.Execute(Generator generator) {
            ILoopStatement loop = generator.GetLoop(label, position);

            uint start = generator.GetByteCount();
            UndertaleInstruction instruction = generator.Branch();
            loop.PromiseBreak(start, instruction);
        }
    }
}
