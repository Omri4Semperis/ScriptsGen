namespace ScriptsGen;

public class EventCsvReader
{
    public List<Event> ReadEvents(string csvFilePath)
    {
        var events = new List<Event>();
        
        if (!File.Exists(csvFilePath))
        {
            throw new FileNotFoundException($"CSV file not found: {csvFilePath}");
        }

        var lines = File.ReadAllLines(csvFilePath);
        
        // Skip header row
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                events.Add(new Event
                {
                    Log = parts[0].Trim(),
                    EventId = parts[1].Trim(),
                    EventDescription = parts[2].Trim()
                });
            }
        }

        return events;
    }
}
