namespace ScriptsGen;

public partial class MainForm : Form
{
    private List<Event> _events = new();
    private List<CheckBox> _eventCheckBoxes = new();
    private Button _continueButton = new();
    private Panel _eventsPanel = new();
    private Label _welcomeLabel = new();

    public List<Event> SelectedEvents
    {
        get
        {
            var selectedEvents = new List<Event>();
            for (int i = 0; i < _eventCheckBoxes.Count; i++)
            {
                if (_eventCheckBoxes[i].Checked)
                {
                    selectedEvents.Add(_events[i]);
                }
            }
            return selectedEvents;
        }
    }

    public MainForm()
    {
        InitializeComponent();
        LoadEvents();
        CreateUI();
    }

    private void InitializeComponent()
    {
        Text = "Script Generator - Event Selection";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F);
    }

    private void LoadEvents()
    {
        try
        {
            var csvReader = new EventCsvReader();
            var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Knowledge", "events.csv");
            
            // If not found in bin directory, try the source directory
            if (!File.Exists(csvPath))
            {
                csvPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Knowledge", "events.csv");
            }
            
            _events = csvReader.ReadEvents(csvPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _events = new List<Event>(); // Fallback to empty list
        }
    }

    private void CreateUI()
    {
        SuspendLayout();

        // Welcome label
        _welcomeLabel.Text = "Welcome to Script Generator!\n\nPlease select the events for which you want to generate scripts:";
        _welcomeLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        _welcomeLabel.Location = new Point(20, 20);
        _welcomeLabel.Size = new Size(760, 60);
        _welcomeLabel.TextAlign = ContentAlignment.TopLeft;
        Controls.Add(_welcomeLabel);

        // Events panel with scroll
        _eventsPanel.Location = new Point(20, 90);
        _eventsPanel.Size = new Size(760, 450);
        _eventsPanel.AutoScroll = true;
        _eventsPanel.BorderStyle = BorderStyle.FixedSingle;
        Controls.Add(_eventsPanel);

        // Create checkboxes for each event
        int yPosition = 10;
        foreach (var eventItem in _events)
        {
            var checkBox = new CheckBox();
            checkBox.Text = $"[{eventItem.Log}] Event {eventItem.EventId}: {eventItem.EventDescription}";
            checkBox.Location = new Point(10, yPosition);
            checkBox.Size = new Size(720, 25);
            checkBox.Font = new Font("Segoe UI", 9F);
            
            // Color code based on log type
            if (eventItem.Log.Equals("Security", StringComparison.OrdinalIgnoreCase))
            {
                checkBox.ForeColor = Color.DarkBlue;
            }
            else if (eventItem.Log.Equals("System", StringComparison.OrdinalIgnoreCase))
            {
                checkBox.ForeColor = Color.DarkGreen;
            }

            _eventCheckBoxes.Add(checkBox);
            _eventsPanel.Controls.Add(checkBox);
            
            yPosition += 30;
        }

        // Continue button
        _continueButton.Text = "Continue";
        _continueButton.Location = new Point(700, 550);
        _continueButton.Size = new Size(80, 30);
        _continueButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _continueButton.BackColor = Color.LightBlue;
        _continueButton.Click += ContinueButton_Click;
        Controls.Add(_continueButton);

        ResumeLayout();
    }

    private void ContinueButton_Click(object? sender, EventArgs e)
    {
        var selectedEvents = SelectedEvents;
        
        if (selectedEvents.Count == 0)
        {
            MessageBox.Show("Please select at least one event.", "No Events Selected", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var message = $"You have selected {selectedEvents.Count} event(s):\n\n";
        foreach (var evt in selectedEvents)
        {
            message += $"• {evt}\n";
        }
        message += "\nScript generation will continue with these events.";

        MessageBox.Show(message, "Selected Events", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        // TODO: Continue to next step of the application
        // For now, we'll just show the selection
    }
}
