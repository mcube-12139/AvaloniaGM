using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.TypeNodes {
    internal interface ITypeNode {
        IType GetSharkType(Generator generator);
    }
}
