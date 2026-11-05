using TypeScript.Intermediate.Instruction;

namespace TypeScript.Statement;

internal interface ILoopStatement: IStatement {
    int GetStart();
    void PromiseBreak(JumpInstruction instruction);
}
