using System.Collections.Generic;
using UndertaleModLib.Models;

namespace AvaloniaGM.Services;

internal class LoopContext(uint start) {
    internal readonly uint start = start;
    internal readonly List<(uint start, UndertaleInstruction instruction)> promises = [];
}
