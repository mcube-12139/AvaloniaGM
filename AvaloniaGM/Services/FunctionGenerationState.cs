using System.Collections.Generic;
using TypeScript.Statement;
using TypeScript.Symbol;
using UndertaleModLib.Models;
using static UndertaleModLib.Models.UndertaleInstruction;

namespace AvaloniaGM.Services;

internal class FunctionGenerationState(FunctionSymbol? funSymbol, uint byteCount, List<UndertaleInstruction> instructions, Stack<DataType> types, uint nextLocalIndex, uint nextParameterIndex, UndertaleCodeLocals codeLocals, Dictionary<ILoopStatement, LoopContext> loopContexts) {
    internal readonly FunctionSymbol? funSymbol = funSymbol;
    internal readonly uint byteCount = byteCount;
    internal readonly List<UndertaleInstruction> instructions = instructions;
    internal readonly Stack<DataType> types = types;
    internal readonly uint nextLocalIndex = nextLocalIndex;
    internal readonly uint nextParameterIndex = nextParameterIndex;
    internal readonly UndertaleCodeLocals CodeLocals = codeLocals;
    internal readonly Dictionary<ILoopStatement, LoopContext> LoopContexts = loopContexts;
}
