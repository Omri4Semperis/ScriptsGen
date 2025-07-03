namespace ScriptsGen;

public class Script
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Dictionary<string, bool> EventTriggers { get; set; } = new();

    public override string ToString()
    {
        return $"{Name}: {Description}";
    }
}
