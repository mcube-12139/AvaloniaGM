using AvaloniaGM.TypeScript.Patterns;
using AvaloniaGM.TypeScript.TypeNodes;
using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Parameters {
    internal class PatternParameter(TextPosition position, IPattern pattern, ITypeNode typeNode) : IParameter {
        void IParameter.AddVariable(Generator generator) {
            pattern.AddParameter(typeNode, generator);
        }

        IType IParameter.GetSharkType(Generator generator) =>
            typeNode.GetSharkType(generator);
    }
}
