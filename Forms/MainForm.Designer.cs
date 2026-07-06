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

            this.buttonAddDestination = new Button
            {
                Text = "+  Thêm destination",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(44, 44, 42),
                ForeColor = System.Drawing.Color.White,
                Margin = new Padding(0, 0, 8, 0),
                Padding = new Padding(6, 2, 6, 2)
            };
            this.buttonAddDestination.FlatAppearance.BorderSize = 0;
            this.buttonAddDestination.Click += buttonAddDestination_Click;

            this.buttonViewLogs = new Button
            {
                Text = "Xem log",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                Padding = new Padding(6, 2, 6, 2)
            };
            this.buttonViewLogs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(224, 222, 217);
            this.buttonViewLogs.Click += buttonViewLogs_Click;

            buttonPanel.Controls.Add(this.buttonAddDestination);
            buttonPanel.Controls.Add(this.buttonViewLogs);

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

        private Button buttonAddDestination = null!;
        private Button buttonViewLogs = null!;
        private FlowLayoutPanel flowLayoutPanelCards = null!;
    }
}
