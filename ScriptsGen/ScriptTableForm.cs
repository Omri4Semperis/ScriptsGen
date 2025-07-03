namespace ScriptsGen;

public partial class ScriptTableForm : Form
{
    private DataGridView _scriptTable = new();
    private List<Script> _scripts = new();
    private List<Event> _selectedEvents = new();
    private List<Event> _allEvents = new();

    public ScriptTableForm(List<Script> scripts, List<Event> selectedEvents, List<Event> allEvents)
    {
        _scripts = scripts;
        _selectedEvents = selectedEvents;
        _allEvents = allEvents;
        InitializeComponent();
        CreateUI();
        PopulateTable();
    }

    private void InitializeComponent()
    {
        Text = "Script Generator - Script Selection";
        Size = new Size(1200, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9F);
    }

    private void CreateUI()
    {
        SuspendLayout();

        // Title label
        var titleLabel = new Label();
        titleLabel.Text = $"Scripts that can trigger at least one of your selected events:\n" +
                         $"Selected Events: {string.Join(", ", _selectedEvents.Select(e => e.EventId))}";
        titleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        titleLabel.Location = new Point(20, 20);
        titleLabel.Size = new Size(1160, 50);
        titleLabel.TextAlign = ContentAlignment.TopLeft;
        Controls.Add(titleLabel);

        // Instructions label
        var instructionLabel = new Label();
        instructionLabel.Text = "• Events you requested are highlighted in yellow\n" +
                               "• Click on a script name to open it in the default text editor\n" +
                               "• TRUE means the script triggers that event, empty cells mean it doesn't";
        instructionLabel.Font = new Font("Segoe UI", 9F);
        instructionLabel.Location = new Point(20, 80);
        instructionLabel.Size = new Size(1160, 60);
        instructionLabel.TextAlign = ContentAlignment.TopLeft;
        Controls.Add(instructionLabel);

        // DataGridView for the table
        _scriptTable.Location = new Point(20, 150);
        _scriptTable.Size = new Size(1160, 500);
        _scriptTable.AllowUserToAddRows = false;
        _scriptTable.AllowUserToDeleteRows = false;
        _scriptTable.ReadOnly = true;
        _scriptTable.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _scriptTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        _scriptTable.ScrollBars = ScrollBars.Both;
        _scriptTable.CellClick += ScriptTable_CellClick;
        
        Controls.Add(_scriptTable);

        ResumeLayout();
    }

    private void PopulateTable()
    {
        // Get all unique event IDs from all scripts
        var allEventIds = new HashSet<string>();
        foreach (var script in _scripts)
        {
            foreach (var eventId in script.EventTriggers.Keys)
            {
                allEventIds.Add(eventId);
            }
        }

        // Sort event IDs numerically
        var sortedEventIds = allEventIds.OrderBy(id => int.TryParse(id, out int numId) ? numId : int.MaxValue).ToList();

        // Create columns
        _scriptTable.Columns.Add("ScriptNumber", "Script #");
        _scriptTable.Columns.Add("ScriptName", "Script Name");
        _scriptTable.Columns.Add("Description", "Script Description");

        // Add event columns
        foreach (var eventId in sortedEventIds)
        {
            var column = new DataGridViewColumn(new DataGridViewTextBoxCell());
            column.Name = $"Event_{eventId}";
            column.HeaderText = $"Event {eventId}";
            column.Width = 80;
            _scriptTable.Columns.Add(column);
        }

        // Set column widths
        if (_scriptTable.Columns["ScriptNumber"] != null)
            _scriptTable.Columns["ScriptNumber"]!.Width = 70;
        if (_scriptTable.Columns["ScriptName"] != null)
            _scriptTable.Columns["ScriptName"]!.Width = 100;
        if (_scriptTable.Columns["Description"] != null)
            _scriptTable.Columns["Description"]!.Width = 300;

        // Add rows
        for (int i = 0; i < _scripts.Count; i++)
        {
            var script = _scripts[i];
            var row = new object[_scriptTable.Columns.Count];
            
            row[0] = i + 1; // Script number
            row[1] = script.Name; // Script name
            row[2] = script.Description; // Description

            // Fill event columns
            for (int j = 3; j < _scriptTable.Columns.Count; j++)
            {
                var column = _scriptTable.Columns[j];
                var eventId = column.Name.Replace("Event_", "");
                
                if (script.EventTriggers.ContainsKey(eventId))
                {
                    row[j] = script.EventTriggers[eventId] ? "TRUE" : "";
                }
                else
                {
                    row[j] = "";
                }
            }

            _scriptTable.Rows.Add(row);
        }

        // Highlight requested event columns in yellow
        var selectedEventIds = _selectedEvents.Select(e => e.EventId).ToHashSet();
        foreach (DataGridViewColumn column in _scriptTable.Columns)
        {
            if (column?.Name?.StartsWith("Event_") == true)
            {
                var eventId = column.Name.Replace("Event_", "");
                if (selectedEventIds.Contains(eventId))
                {
                    column.DefaultCellStyle.BackColor = Color.Yellow;
                    if (column.HeaderCell?.Style != null)
                    {
                        column.HeaderCell.Style.BackColor = Color.Gold;
                        column.HeaderCell.Style.Font = new Font(_scriptTable.Font, FontStyle.Bold);
                    }
                }
            }
        }

        // Style the script name column to look clickable
        var scriptNameColumn = _scriptTable.Columns["ScriptName"];
        if (scriptNameColumn != null)
        {
            scriptNameColumn.DefaultCellStyle.ForeColor = Color.Blue;
            scriptNameColumn.DefaultCellStyle.Font = new Font(_scriptTable.Font, FontStyle.Underline);
        }
    }

    private void ScriptTable_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && _scriptTable.Columns.Count > e.ColumnIndex)
        {
            var column = _scriptTable.Columns[e.ColumnIndex];
            
            // Check if user clicked on script name column
            if (column?.Name == "ScriptName" && e.RowIndex < _scripts.Count)
            {
                var script = _scripts[e.RowIndex];
                OpenScriptInEditor(script);
            }
        }
    }

    private void OpenScriptInEditor(Script script)
    {
        try
        {
            if (File.Exists(script.FilePath))
            {
                // Open the script file in the default text editor
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = script.FilePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"Script file not found: {script.FilePath}", "File Not Found", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error opening script file: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
