namespace DataBackupTool.Forms
{
    public partial class LogViewerForm : Form
    {
        private readonly string _logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
        private ComboBox comboBoxLogFiles = null!;
        private RichTextBox textBoxLogContent = null!;
        private Button buttonRefresh = null!;
        private System.Windows.Forms.Timer? _refreshTimer;

        public LogViewerForm()
        {
            InitializeComponent();
            LoadLogFileList();
        }

        private void InitializeComponent()
        {
            this.Text = "Lịch sử Backup (Log)";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormClosing += (_, _) => _refreshTimer?.Stop();

            var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };

            comboBoxLogFiles = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
            comboBoxLogFiles.SelectedIndexChanged += (_, _) => LoadLogContent();

            buttonRefresh = new Button { Text = "Làm mới", AutoSize = true };
            buttonRefresh.Click += (_, _) => { LoadLogFileList(); LoadLogContent(); };

            topPanel.Controls.Add(comboBoxLogFiles);
            topPanel.Controls.Add(buttonRefresh);

            textBoxLogContent = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                Font = new Font("Consolas", 9)
            };

            _refreshTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _refreshTimer.Tick += (_, _) => { LoadLogFileList(); LoadLogContent(); };
            _refreshTimer.Start();

            this.Controls.Add(textBoxLogContent);
            this.Controls.Add(topPanel);
        }

        private void LoadLogFileList()
        {
            comboBoxLogFiles.Items.Clear();

            if (!Directory.Exists(_logDir))
            {
                return;
            }

            var files = Directory.GetFiles(_logDir, "log_*.txt")
                .OrderByDescending(f => f)
                .Select(Path.GetFileName)
                .ToArray();

            comboBoxLogFiles.Items.AddRange(files!);

            if (comboBoxLogFiles.Items.Count > 0)
            {
                comboBoxLogFiles.SelectedIndex = 0;
            }
        }

        private void LoadLogContent()
        {
            if (comboBoxLogFiles.SelectedItem is not string fileName)
            {
                textBoxLogContent.Text = "";
                return;
            }

            var path = Path.Combine(_logDir, fileName);
            var content = File.Exists(path) ? File.ReadAllText(path) : "";
            textBoxLogContent.Clear();
            textBoxLogContent.SelectionColor = Color.Black;

            if (string.IsNullOrWhiteSpace(content))
            {
                textBoxLogContent.Text = "";
                return;
            }

            foreach (var line in content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.Contains("[Realtime]", StringComparison.OrdinalIgnoreCase) || line.Contains("[Scheduler]", StringComparison.OrdinalIgnoreCase))
                {
                    textBoxLogContent.SelectionColor = Color.DarkBlue;
                }
                else if (line.Contains("Lỗi", StringComparison.OrdinalIgnoreCase) || line.Contains("Error", StringComparison.OrdinalIgnoreCase))
                {
                    textBoxLogContent.SelectionColor = Color.DarkRed;
                }
                else
                {
                    textBoxLogContent.SelectionColor = Color.Green;
                }

                textBoxLogContent.AppendText(line + Environment.NewLine);
            }

            textBoxLogContent.SelectionStart = textBoxLogContent.Text.Length;
            textBoxLogContent.ScrollToCaret();
        }
    }
}
