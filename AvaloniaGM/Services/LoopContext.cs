using System.Collections.Generic;
using UndertaleModLib.Models;

namespace AvaloniaGM.Services;

internal class LoopContext(uint start) {
    internal readonly uint start = start;
    readonly List<(uint start, UndertaleInstruction instruction)> promises = [];

    internal void AddPromise(uint start, UndertaleInstruction instruction) {
        promises.Add((start, instruction));
    }

    internal IEnumerable<(uint start, UndertaleInstruction instruction)> IteratePromise() {
        foreach ((uint start, UndertaleInstruction instruction) in promises) {
            yield return (start, instruction);
        }
    }
}
