namespace ScriptsGen;

public class Event
{
    public string Log { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string EventDescription { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{Log}] Event {EventId}: {EventDescription}";
    }
}
