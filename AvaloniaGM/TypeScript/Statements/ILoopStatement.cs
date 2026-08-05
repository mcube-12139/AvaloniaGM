using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Statements {
    internal interface ILoopStatement: IStatement {
        uint GetStart();
        void PromiseBreak(uint start, UndertaleInstruction instruction);
    }
}
