using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.TypeNodes {
    internal class PathType(TextPosition position, bool fromGlobal, string[] segments) : ITypeNode {
        IType ITypeNode.GetSharkType(Generator generator) {
            return generator
                .GetSymbol(segments[0], position)
                .AsType(position, generator)
                .GetSharkType(position, generator);
        }
    }
}
