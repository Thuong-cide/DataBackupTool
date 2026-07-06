using DataBackupTool.Controls;

namespace DataBackupTool.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Data Backup Tool";
            this.Size = new System.Drawing.Size(920, 560);
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 243);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += MainForm_FormClosing;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                ColumnCount = 1,
                RowCount = 2
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 12)
            };

            this.checkBoxStartWithWindows = new CheckBox
            {
                Text = "Khởi động cùng Windows",
                AutoSize = true,
                Margin = new Padding(16, 8, 0, 0)
            };

            this.buttonAddDestination = new RoundedButton
            {
                Text = "+  Thêm destination",
                Size = new Size(150, 34),
                Variant = ButtonVariant.Primary,
                Margin = new Padding(0, 0, 8, 0)
            };
            this.buttonAddDestination.Click += buttonAddDestination_Click;

            this.buttonViewLogs = new RoundedButton
            {
                Text = "Xem log",
                Size = new Size(90, 34),
                Variant = ButtonVariant.Secondary
            };
            this.buttonViewLogs.Click += buttonViewLogs_Click;

            buttonPanel.Controls.Add(this.buttonAddDestination);
            buttonPanel.Controls.Add(this.buttonViewLogs);
            buttonPanel.Controls.Add(this.checkBoxStartWithWindows);

            this.flowLayoutPanelCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 243)
            };

            layout.Controls.Add(buttonPanel, 0, 0);
            layout.Controls.Add(this.flowLayoutPanelCards, 0, 1);
            this.Controls.Add(layout);
        }

        private RoundedButton buttonAddDestination = null!;
        private RoundedButton buttonViewLogs = null!;
        private CheckBox checkBoxStartWithWindows = null!;
        private FlowLayoutPanel flowLayoutPanelCards = null!;
    }
}
