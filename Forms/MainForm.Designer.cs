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
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += MainForm_FormClosing;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 1,
                RowCount = 2
            };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 8)
            };

            this.buttonAddDestination = new Button { Text = "Thêm Destination", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
            this.buttonAddDestination.Click += buttonAddDestination_Click;
            this.buttonRunSelected = new Button { Text = "Chạy ngay", AutoSize = true };
            this.buttonRunSelected.Click += buttonRunSelected_Click;

            buttonPanel.Controls.Add(this.buttonAddDestination);
            buttonPanel.Controls.Add(this.buttonRunSelected);

            this.dataGridViewDestinations = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dataGridViewDestinations.Columns.Add("Id", "Id");
            this.dataGridViewDestinations.Columns.Add("Name", "Tên");
            this.dataGridViewDestinations.Columns.Add("DestinationPath", "Đích");
            this.dataGridViewDestinations.Columns.Add("SourceCount", "Số nguồn");
            this.dataGridViewDestinations.Columns.Add("Mode", "Chế độ");
            this.dataGridViewDestinations.Columns.Add("Schedule", "Lịch");

            layout.Controls.Add(buttonPanel, 0, 0);
            layout.Controls.Add(this.dataGridViewDestinations, 0, 1);
            this.Controls.Add(layout);
        }

        private Button buttonAddDestination = null!;
        private Button buttonRunSelected = null!;
        private DataGridView dataGridViewDestinations = null!;
    }
}
