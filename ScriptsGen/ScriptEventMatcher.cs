namespace ScriptsGen;

public class ScriptEventMatcher
{
    public List<Script> LoadScriptsAndMatching(string scriptsPath, string matchingCsvPath)
    {
        var scripts = new List<Script>();
        
        // Read the CSV file to get script-event mappings
        var lines = File.ReadAllLines(matchingCsvPath);
        if (lines.Length < 2) return scripts;

        // Parse header to get event IDs
        var header = lines[0].Split(',');
        var eventIds = header.Skip(1).ToArray(); // Skip "Script" column

        // Parse each script row
        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            if (values.Length < 2) continue;

            var scriptName = values[0];
            var script = new Script
            {
                Name = scriptName,
                FilePath = Path.Combine(scriptsPath, "TheScripts", $"{scriptName}.txt"),
                EventTriggers = new Dictionary<string, bool>()
            };

            // Load script description from file
            script.Description = GetScriptDescription(script.FilePath);

            // Parse event triggers (1 = true, 0 = false)
            for (int j = 1; j < values.Length && j - 1 < eventIds.Length; j++)
            {
                var eventId = eventIds[j - 1];
                var triggers = values[j] == "1";
                script.EventTriggers[eventId] = triggers;
            }

            scripts.Add(script);
        }

        return scripts;
    }

    private string GetScriptDescription(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return "Script file not found";

            var lines = File.ReadAllLines(filePath);
            
            // Look for the first comment line that describes what the script does
            foreach (var line in lines.Take(10)) // Check first 10 lines
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("#") && trimmed.Length > 1)
                {
                    var description = trimmed.Substring(1).Trim();
                    if (description.ToLower().Contains("script to") || 
                        description.ToLower().Contains("trigger") ||
                        description.Length > 20) // Likely a description
                    {
                        return description.Length > 100 ? description.Substring(0, 97) + "..." : description;
                    }
                }
            }

            return "PowerShell script";
        }
        catch
        {
            return "Description unavailable";
        }
    }

    public List<Script> FilterScriptsForEvents(List<Script> allScripts, List<Event> selectedEvents)
    {
        var selectedEventIds = selectedEvents.Select(e => e.EventId).ToList();
        
        return allScripts.Where(script => 
            selectedEventIds.Any(eventId => 
                script.EventTriggers.ContainsKey(eventId) && 
                script.EventTriggers[eventId]
            )
        ).ToList();
    }

    public List<Script> CheckForPerfectMatches(List<Script> allScripts, List<Event> selectedEvents)
    {
        var selectedEventIds = selectedEvents.Select(e => e.EventId).ToHashSet();
        
        return allScripts.Where(script =>
            selectedEventIds.All(eventId =>
                script.EventTriggers.ContainsKey(eventId) &&
                script.EventTriggers[eventId]
            )
        ).ToList();
    }
}
