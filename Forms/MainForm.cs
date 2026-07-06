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
            flowLayoutPanelCards.Controls.Clear();
            var destinations = _configService.GetAllDestinations();

            foreach (var destination in destinations)
            {
                var card = new DestinationCard(destination);
                card.RunClicked += RunDestination;
                card.EditClicked += EditDestination;
                card.DeleteClicked += DeleteDestination;
                flowLayoutPanelCards.Controls.Add(card);
            }
        }

        private void RunDestination(BackupDestination destination)
        {
            _scheduler.RunNow(destination);
            MessageBox.Show(this, "Đã chạy backup/sync cho destination đã chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditDestination(BackupDestination destination)
        {
            using var form = new DestinationEditForm(destination);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _configService.UpdateDestination(form.Destination);
                _realtimeWatcher.StartWatching(_configService.GetAllDestinations());
                LoadDestinations();
            }
        }

        private void DeleteDestination(BackupDestination destination)
        {
            var confirm = MessageBox.Show(this, $"Xóa destination \"{destination.Name}\"? Hành động này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _configService.RemoveDestination(destination.Id);
            _realtimeWatcher.StartWatching(_configService.GetAllDestinations());
            LoadDestinations();
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

        private void buttonViewLogs_Click(object sender, EventArgs e)
        {
            using var form = new LogViewerForm();
            form.ShowDialog(this);
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
