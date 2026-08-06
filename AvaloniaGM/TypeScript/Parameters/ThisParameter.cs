using AvaloniaGM.TypeScript.Types;

namespace AvaloniaGM.TypeScript.Parameters {
    internal class ThisParameter(TextPosition position) : IParameter {
        void IParameter.AddVariable(Generator generator) {
            // 无事可做
        }

        IType IParameter.GetSharkType(Generator generator) {
            throw new System.NotImplementedException();
        }
    }
}
