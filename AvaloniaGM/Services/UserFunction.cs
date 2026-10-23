using UndertaleModLib.Models;

namespace AvaloniaGM.Services;

internal class UserFunction(int scriptId, UndertaleScript script, UndertaleFunction fun, UndertaleCode code) {
    internal readonly int scriptId = scriptId;
    internal readonly UndertaleScript script = script;
    internal readonly UndertaleFunction fun = fun;
    internal readonly UndertaleCode code = code;
}
