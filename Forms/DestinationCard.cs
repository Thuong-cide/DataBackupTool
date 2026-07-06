using System.Drawing.Drawing2D;
using DataBackupTool.Controls;
using DataBackupTool.Models;

namespace DataBackupTool.Forms
{
    public class DestinationCard : Panel
    {
        public BackupDestination Destination { get; }
        public event Action<BackupDestination>? RunClicked;
        public event Action<BackupDestination>? EditClicked;
        public event Action<BackupDestination>? DeleteClicked;

        private static readonly Color CardBg = Color.White;
        private static readonly Color BorderColor = Color.FromArgb(224, 222, 217);
        private static readonly Color TextPrimary = Color.FromArgb(44, 44, 42);
        private static readonly Color TextSecondary = Color.FromArgb(95, 94, 90);
        private static readonly Color TextMuted = Color.FromArgb(136, 135, 128);
        private static readonly Color IconBg = Color.FromArgb(241, 239, 232);

        public DestinationCard(BackupDestination destination)
        {
            Destination = destination;
            Size = new Size(212, 196);
            Margin = new Padding(6);
            BackColor = CardBg;
            DoubleBuffered = true;

            BuildContent();
            ApplyRoundedRegion();
        }

        private void ApplyRoundedRegion()
        {
            using var path = RoundedRect(new Rectangle(0, 0, Width, Height), 12);
            Region = new Region(path);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(BorderColor, 1);
            using var path = RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 12);
            e.Graphics.DrawPath(pen, path);
        }

        private void BuildContent()
        {
            var isSync = Destination.Mode == BackupMode.Sync;

            var iconBox = new Panel { Size = new Size(36, 36), Location = new Point(14, 14), BackColor = IconBg };
            iconBox.Controls.Add(new Label
            {
                Text = isSync ? "↻" : "▤",
                Font = new Font("Segoe UI", 14),
                ForeColor = TextSecondary,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });

            var statusDot = new Panel { Size = new Size(8, 8), Location = new Point(Width - 24, 20), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            statusDot.Paint += (_, e) =>
            {
                var color = Destination.IsRealtimeEnabled ? Color.FromArgb(55, 138, 221)
                    : Destination.IsScheduleEnabled ? Color.FromArgb(99, 153, 34)
                    : Color.FromArgb(180, 178, 169);
                using var b = new SolidBrush(color);
                e.Graphics.FillEllipse(b, 0, 0, 8, 8);
            };

            var nameLabel = new Label
            {
                Text = Destination.Name,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(58, 16),
                Size = new Size(120, 18),
                AutoEllipsis = true
            };
            var modeLabel = new Label
            {
                Text = isSync ? "Sync" : "Backup",
                Font = new Font("Segoe UI", 8f),
                ForeColor = TextMuted,
                Location = new Point(58, 34),
                Size = new Size(120, 16)
            };

            var pathLabel = new Label
            {
                Text = Destination.DestinationPath,
                Font = new Font("Segoe UI", 8f),
                ForeColor = TextSecondary,
                Location = new Point(14, 62),
                Size = new Size(184, 16),
                AutoEllipsis = true
            };
            var scheduleLabel = new Label
            {
                Text = Destination.IsScheduleEnabled ? $"Lịch: {Destination.ScheduleTime} hằng ngày" : "Lịch: tắt",
                Font = new Font("Segoe UI", 8f),
                ForeColor = TextMuted,
                Location = new Point(14, 80),
                Size = new Size(184, 16)
            };

            var separator = new Panel { BackColor = BorderColor, Height = 1, Location = new Point(14, 110), Size = new Size(184, 1) };

            var buttonRun = MakeButton("Chạy", new Point(14, 122), 56, ButtonVariant.Primary);
            buttonRun.Click += (_, _) => RunClicked?.Invoke(Destination);
            var buttonEdit = MakeButton("Sửa", new Point(76, 122), 56, ButtonVariant.Secondary);
            buttonEdit.Click += (_, _) => EditClicked?.Invoke(Destination);
            var buttonDelete = MakeButton("Xóa", new Point(138, 122), 60, ButtonVariant.Danger);
            buttonDelete.Click += (_, _) => DeleteClicked?.Invoke(Destination);

            Controls.Add(iconBox);
            Controls.Add(statusDot);
            Controls.Add(nameLabel);
            Controls.Add(modeLabel);
            Controls.Add(pathLabel);
            Controls.Add(scheduleLabel);
            Controls.Add(separator);
            Controls.Add(buttonRun);
            Controls.Add(buttonEdit);
            Controls.Add(buttonDelete);
        }

        private RoundedButton MakeButton(string text, Point location, int width, ButtonVariant variant = ButtonVariant.Secondary)
        {
            return new RoundedButton
            {
                Text = text,
                Location = location,
                Size = new Size(width, 28),
                Variant = variant
            };
        }
    }
}
