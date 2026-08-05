using AvaloniaGM.TypeScript.Parameters;
using AvaloniaGM.TypeScript.Statements;
using AvaloniaGM.TypeScript.TypeNodes;

namespace AvaloniaGM.TypeScript.Items {
    internal class FunctionItem(TextPosition position, string name, IParameter[] parameters, ITypeNode? resultType, BlockStatement body): IItem {
    }
}
