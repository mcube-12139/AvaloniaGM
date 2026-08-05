using AvaloniaGM.TypeScript.Expressions;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript.Statements {
    internal class IfStatement(TextPosition position, IExpression condition, BlockStatement body, IStatement? elseBody) : IStatement {
        void IStatement.Execute(Generator generator) {
            condition.Evaluate(generator);
            uint bfStart = generator.GetByteCount();
            UndertaleInstruction bfInstruction = generator.BranchFalse();

            body.Execute(generator);

            if (elseBody != null) {
                uint branchStart = generator.GetByteCount();
                UndertaleInstruction branchInstruction = generator.Branch();

                bfInstruction.JumpOffset = (int)(generator.GetByteCount() - bfStart) / 4;

                elseBody.Execute(generator);

                branchInstruction.JumpOffset = (int)(generator.GetByteCount() - branchStart) / 4;
            } else {
                bfInstruction.JumpOffset = (int)(generator.GetByteCount() - bfStart) / 4;
            }
        }
    }
}
