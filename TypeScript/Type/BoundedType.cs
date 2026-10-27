using TypeScript.ExceptionSpace;
using TypeScript.Expression;
using TypeScript.Field;

namespace TypeScript.Type;

public class BoundedType: IType {
    IConcept[] bounds;

    internal BoundedType(IConcept[] bounds) {
        this.bounds = bounds;
    }

    public string GetAppearance() {
        return string.Join(" & ", bounds);
    }
}
