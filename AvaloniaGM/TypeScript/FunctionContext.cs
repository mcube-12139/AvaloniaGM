using AvaloniaGM.TypeScript.Types;
using System.Collections.Generic;
using UndertaleModLib.Models;

namespace AvaloniaGM.TypeScript {
    internal class FunctionContext(
        List<UndertaleInstruction> instructions,
        uint byteCount,
        IType resultType,
        UndertaleCodeLocals codeLocals,
        uint nextLocalId
    ) {
        internal readonly List<UndertaleInstruction> instructions = instructions;
        internal readonly uint byteCount = byteCount;
        internal readonly IType resultType = resultType;
        internal readonly UndertaleCodeLocals codeLocals = codeLocals;
        internal readonly uint nextLocalId = nextLocalId;
    }
}
