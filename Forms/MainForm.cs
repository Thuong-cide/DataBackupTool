using DataBackupTool.Models;
using DataBackupTool.Services;

namespace DataBackupTool.Forms
{
    public partial class MainForm : Form
    {
        private readonly ConfigService _configService = new();
        private readonly SchedulerService _scheduler;
        private readonly RealtimeWatcherService _realtimeWatcher;
        private readonly NotifyIcon _notifyIcon = new();

        public MainForm(SchedulerService scheduler, RealtimeWatcherService realtimeWatcher)
        {
            InitializeComponent();
            _scheduler = scheduler;
            _realtimeWatcher = realtimeWatcher;
            InitializeNotifyIcon();
            _realtimeWatcher.BackupCompleted += OnRealtimeBackupCompleted;
            LoadDestinations();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon.Icon = SystemIcons.Application;
            _notifyIcon.Text = "Data Backup Tool";
            _notifyIcon.Visible = false;
            _notifyIcon.Click += (_, _) => ShowWindow();
        }

        private void OnRealtimeBackupCompleted(string destinationName, BackupResult result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnRealtimeBackupCompleted(destinationName, result)));
                return;
            }

            _notifyIcon.Visible = true;
            _notifyIcon.BalloonTipTitle = $"Đã tự động backup: {destinationName}";
            _notifyIcon.BalloonTipText = $"Copy: {result.FilesCopied}, Bỏ qua: {result.FilesSkipped}, Lỗi: {result.Errors.Count}";
            _notifyIcon.ShowBalloonTip(3000);

            LoadDestinations();
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
                _realtimeWatcher.StartWatching(_configService.GetAllDestinations());
                LoadDestinations();
            }
        }

        private void buttonEditDestination_Click(object sender, EventArgs e)
        {
            if (dataGridViewDestinations.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Vui lòng chọn một destination để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedId = dataGridViewDestinations.SelectedRows[0].Cells[0].Value?.ToString();
            var destination = _configService.GetAllDestinations().FirstOrDefault(d => d.Id == selectedId);
            if (destination == null) return;

            using var form = new DestinationEditForm(destination);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _configService.UpdateDestination(form.Destination);
                _realtimeWatcher.StartWatching(_configService.GetAllDestinations());
                LoadDestinations();
            }
        }

        private void buttonDeleteDestination_Click(object sender, EventArgs e)
        {
            if (dataGridViewDestinations.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Vui lòng chọn một destination để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedId = dataGridViewDestinations.SelectedRows[0].Cells[0].Value?.ToString();
            var name = dataGridViewDestinations.SelectedRows[0].Cells[1].Value?.ToString();

            var confirm = MessageBox.Show(this, $"Xóa destination \"{name}\"? Hành động này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes || selectedId == null) return;

            _configService.RemoveDestination(selectedId);
            _realtimeWatcher.StartWatching(_configService.GetAllDestinations());
            LoadDestinations();
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

        private void buttonViewLogs_Click(object sender, EventArgs e)
        {
            using var form = new LogViewerForm();
            form.ShowDialog(this);
        }

        private void dataGridViewDestinations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            buttonEditDestination_Click(sender, EventArgs.Empty);
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
