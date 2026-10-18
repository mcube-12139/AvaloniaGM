using UndertaleModLib.Models;

namespace AvaloniaGM.Services;

internal class UserFunction(UndertaleScript script, UndertaleFunction fun, UndertaleCode code) {
    internal readonly UndertaleScript script = script;
    internal readonly UndertaleFunction fun = fun;
    internal readonly UndertaleCode code = code;
}
