using System.Drawing.Drawing2D;

namespace DataBackupTool.Controls
{
    public enum ButtonVariant { Primary, Secondary, Danger }

    public class RoundedButton : Button
    {
        public ButtonVariant Variant { get; set; } = ButtonVariant.Secondary;

        private bool _hover;
        private bool _pressed;

        private static readonly Color PrimaryBg = Color.FromArgb(44, 44, 42);
        private static readonly Color PrimaryBgHover = Color.FromArgb(65, 65, 62);
        private static readonly Color PrimaryBgPressed = Color.FromArgb(30, 30, 28);
        private static readonly Color BorderColor = Color.FromArgb(224, 222, 217);
        private static readonly Color SecondaryHoverBg = Color.FromArgb(241, 239, 232);
        private static readonly Color DangerText = Color.FromArgb(163, 45, 45);
        private static readonly Color DangerHoverBg = Color.FromArgb(250, 236, 231);
        private static readonly Color TextPrimary = Color.FromArgb(44, 44, 42);

        public RoundedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9f);
            Height = 32;
            BackColor = Color.Transparent;

            MouseEnter += (_, _) => { _hover = true; Invalidate(); };
            MouseLeave += (_, _) => { _hover = false; _pressed = false; Invalidate(); };
            MouseDown += (_, _) => { _pressed = true; Invalidate(); };
            MouseUp += (_, _) => { _pressed = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedRect(rect, 8);

            Color bg;
            Color border;
            Color text;

            switch (Variant)
            {
                case ButtonVariant.Primary:
                    bg = _pressed ? PrimaryBgPressed : _hover ? PrimaryBgHover : PrimaryBg;
                    border = bg;
                    text = Color.White;
                    break;
                case ButtonVariant.Danger:
                    bg = _hover ? DangerHoverBg : Color.White;
                    border = BorderColor;
                    text = DangerText;
                    break;
                default:
                    bg = _hover ? SecondaryHoverBg : Color.White;
                    border = BorderColor;
                    text = TextPrimary;
                    break;
            }

            using (var bgBrush = new SolidBrush(bg))
            {
                e.Graphics.FillPath(bgBrush, path);
            }

            using (var borderPen = new Pen(border, 1))
            {
                e.Graphics.DrawPath(borderPen, path);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, rect, text,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
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
    }
}
