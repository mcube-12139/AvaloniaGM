namespace TypeScript.NameSpace;

internal class SpaceState(INameSpace space, BlockSpace? blockSpace) {
    internal readonly INameSpace space = space;
    internal readonly BlockSpace? blockSpace = blockSpace;
}
