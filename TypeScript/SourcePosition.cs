namespace TypeScript;

public struct SourcePosition(string source, int line, int column) {
    public string source = source;
    public int line = line;
    public int column = column;
}
