namespace TypeScript;

internal class IfFlowContext {
    internal FlowContext ifContext;
    internal FlowContext elseContext;
    internal FlowContext nowContext;

    internal IfFlowContext() {
        ifContext = new();
        elseContext = new();
        nowContext = ifContext;
    }
}
