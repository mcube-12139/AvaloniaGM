using AvaloniaGM.TypeScript.Expressions;
using System.Collections.Generic;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Statements {
    internal class WhileStatement(TextPosition position, string? label, IExpression condition, BlockStatement body) : ILoopStatement {
        uint startOffset;
        List<(uint start, UndertaleInstruction instruction)> breakPromises = [];
        
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

            uint endOffset = generator.GetByteCount();
            bfInstruction.JumpOffset = (int)(endOffset - bfStart) / 4;
            foreach ((uint start, UndertaleInstruction instruction) in breakPromises) {
                instruction.JumpOffset = (int)(endOffset - start) / 4;
            }
        }

        uint ILoopStatement.GetStart() => startOffset;

        void ILoopStatement.PromiseBreak(uint start, UndertaleInstruction instruction) {
            breakPromises.Add((start, instruction));
        }
    }
}
