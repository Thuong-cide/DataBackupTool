using DataBackupTool.Models;
using DataBackupTool.Services;

namespace DataBackupTool.Forms
{
    public partial class MainForm : Form
    {
        private readonly ConfigService _configService = new();
        private readonly SchedulerService _scheduler;
        private readonly NotifyIcon _notifyIcon = new();

        public MainForm(SchedulerService scheduler)
        {
            InitializeComponent();
            _scheduler = scheduler;
            InitializeNotifyIcon();
            LoadDestinations();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.Text = "Data Backup Tool";
            _notifyIcon.Visible = false;
            _notifyIcon.Click += (_, _) => ShowWindow();
        }

        private void LoadDestinations()
        {
            var destinations = _configService.GetAllDestinations();
            dataGridViewDestinations.Rows.Clear();

            foreach (var destination in destinations)
            {
                dataGridViewDestinations.Rows.Add(
                    destination.Id,
                    destination.Name,
                    destination.DestinationPath,
                    destination.SourcePaths.Count,
                    destination.Mode,
                    destination.IsScheduleEnabled ? $"{destination.ScheduleTime}" : "Off");
            }
        }

        private void buttonAddDestination_Click(object sender, EventArgs e)
        {
            using var form = new DestinationEditForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _configService.AddDestination(form.Destination);
                LoadDestinations();
            }
        }

        private void buttonRunSelected_Click(object sender, EventArgs e)
        {
            if (dataGridViewDestinations.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Vui lòng chọn một destination.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedId = dataGridViewDestinations.SelectedRows[0].Cells[0].Value?.ToString();
            var destination = _configService.GetAllDestinations().FirstOrDefault(d => d.Id == selectedId);
            if (destination == null)
            {
                return;
            }

            _scheduler.RunNow(destination);
            MessageBox.Show(this, "Đã chạy backup/sync cho destination đã chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideWindow();
            }
        }

        private void ShowWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void HideWindow()
        {
            Hide();
            _notifyIcon.Visible = true;
        }
    }
}
