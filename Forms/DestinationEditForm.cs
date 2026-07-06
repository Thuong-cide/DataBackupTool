using DataBackupTool.Models;

namespace DataBackupTool.Forms
{
    public partial class DestinationEditForm : Form
    {
        public BackupDestination Destination { get; private set; } = new();

        public DestinationEditForm() : this(null) { }

        public DestinationEditForm(BackupDestination? existing)
        {
            InitializeComponent();
            comboBoxMode.Items.AddRange(new object[] { BackupMode.Backup, BackupMode.Sync });

            if (existing != null)
            {
                Destination = existing;
                textBoxName.Text = existing.Name;
                textBoxDestinationPath.Text = existing.DestinationPath;
                comboBoxMode.SelectedItem = existing.Mode;
                listBoxSources.Items.AddRange(existing.SourcePaths.ToArray());
                checkBoxSchedule.Checked = existing.IsScheduleEnabled;
                checkBoxRealtime.Checked = existing.IsRealtimeEnabled;
                textBoxScheduleTime.Text = existing.ScheduleTime;
            }
            else
            {
                comboBoxMode.SelectedIndex = 0;
            }
        }

        private void buttonBrowseDestination_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxDestinationPath.Text = dialog.SelectedPath;
            }
        }

        private void buttonAddSource_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                listBoxSources.Items.Add(dialog.SelectedPath);
            }
        }

        private void buttonRemoveSource_Click(object sender, EventArgs e)
        {
            if (listBoxSources.SelectedIndex >= 0)
            {
                listBoxSources.Items.RemoveAt(listBoxSources.SelectedIndex);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text) || string.IsNullOrWhiteSpace(textBoxDestinationPath.Text))
            {
                MessageBox.Show(this, "Vui lòng nhập tên và đường dẫn đích.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var id = Destination.Id;
            Destination = new BackupDestination
            {
                Id = id,
                Name = textBoxName.Text,
                DestinationPath = textBoxDestinationPath.Text,
                Mode = (BackupMode)comboBoxMode.SelectedItem!,
                SourcePaths = listBoxSources.Items.Cast<string>().ToList(),
                IsScheduleEnabled = checkBoxSchedule.Checked,
                ScheduleTime = textBoxScheduleTime.Text,
                IsRealtimeEnabled = checkBoxRealtime.Checked
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
