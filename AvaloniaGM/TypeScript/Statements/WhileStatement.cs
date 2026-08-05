using AvaloniaGM.TypeScript.Expressions;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Statements {
    internal class WhileStatement(TextPosition position, string? label, IExpression condition, BlockStatement body) : ILoopStatement {
        uint startOffset;
        
        void IStatement.Execute(Generator generator) {
            startOffset = generator.GetByteCount();

            condition.Evaluate(generator);
            uint bfStart = generator.GetByteCount();
            UndertaleInstruction bfInstruction = generator.BranchFalse();

            body.ExecuteWithStart(generator, () => {
                generator.EnterLoop(label, this);
            });
            generator.LeaveLoop();

            uint branchStart = generator.GetByteCount();
            UndertaleInstruction branchInstruction = generator.Branch();
            branchInstruction.JumpOffset = (int)(startOffset - branchStart) / 4;

            bfInstruction.JumpOffset = (int)(generator.GetByteCount() - bfStart) / 4;
        }

        uint ILoopStatement.GetStart() => startOffset;

        void ILoopStatement.PromiseBreak(uint start, UndertaleInstruction instruction) {
            throw new System.NotImplementedException();
        }
    }
}
