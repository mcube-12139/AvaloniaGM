using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Parameters {
    internal interface IParameter {
        IType GetSharkType(Generator generator);
        void AddVariable(Generator generator);
    }
}
