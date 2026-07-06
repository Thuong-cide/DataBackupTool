namespace DataBackupTool.Forms
{
    partial class DestinationEditForm
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
            this.Text = "Destination";
            this.Size = new System.Drawing.Size(600, 420);
            this.StartPosition = FormStartPosition.CenterParent;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 3 };
            var formLayout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 6 };

            formLayout.Controls.Add(new Label { Text = "Tên", AutoSize = true, Margin = new Padding(0, 0, 8, 6) }, 0, 0);
            this.textBoxName = new TextBox { Width = 320 };
            formLayout.Controls.Add(this.textBoxName, 1, 0);

            formLayout.Controls.Add(new Label { Text = "Đích", AutoSize = true, Margin = new Padding(0, 0, 8, 6) }, 0, 1);
            var destinationPanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            this.textBoxDestinationPath = new TextBox { Width = 250 };
            this.buttonBrowseDestination = new Button { Text = "Browse", AutoSize = true };
            this.buttonBrowseDestination.Click += buttonBrowseDestination_Click;
            destinationPanel.Controls.Add(this.textBoxDestinationPath);
            destinationPanel.Controls.Add(this.buttonBrowseDestination);
            formLayout.Controls.Add(destinationPanel, 1, 1);

            formLayout.Controls.Add(new Label { Text = "Chế độ", AutoSize = true, Margin = new Padding(0, 0, 8, 6) }, 0, 2);
            this.comboBoxMode = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            formLayout.Controls.Add(this.comboBoxMode, 1, 2);

            formLayout.Controls.Add(new Label { Text = "Nguồn", AutoSize = true, Margin = new Padding(0, 0, 8, 6) }, 0, 3);
            var sourcePanel = new FlowLayoutPanel { AutoSize = true };
            this.listBoxSources = new ListBox { Height = 120, Width = 250 };
            var sourceButtons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown };
            this.buttonAddSource = new Button { Text = "Thêm nguồn", AutoSize = true };
            this.buttonAddSource.Click += buttonAddSource_Click;
            this.buttonRemoveSource = new Button { Text = "Xóa nguồn", AutoSize = true };
            this.buttonRemoveSource.Click += buttonRemoveSource_Click;
            sourceButtons.Controls.Add(this.buttonAddSource);
            sourceButtons.Controls.Add(this.buttonRemoveSource);
            sourcePanel.Controls.Add(this.listBoxSources);
            sourcePanel.Controls.Add(sourceButtons);
            formLayout.Controls.Add(sourcePanel, 1, 3);

            this.checkBoxSchedule = new CheckBox { Text = "Bật lịch tự động", AutoSize = true };
            formLayout.Controls.Add(this.checkBoxSchedule, 1, 4);

            formLayout.Controls.Add(new Label { Text = "Giờ", AutoSize = true, Margin = new Padding(0, 0, 8, 6) }, 0, 5);
            this.textBoxScheduleTime = new MaskedTextBox { Mask = "00:00", Width = 80 };
            formLayout.Controls.Add(this.textBoxScheduleTime, 1, 5);

            this.buttonSave = new Button { Text = "Lưu", AutoSize = true, DialogResult = DialogResult.OK };
            this.buttonSave.Click += buttonSave_Click;

            layout.Controls.Add(formLayout, 0, 0);
            layout.Controls.Add(this.buttonSave, 0, 2);
            this.Controls.Add(layout);
        }

        private TextBox textBoxName = null!;
        private TextBox textBoxDestinationPath = null!;
        private Button buttonBrowseDestination = null!;
        private ComboBox comboBoxMode = null!;
        private ListBox listBoxSources = null!;
        private Button buttonAddSource = null!;
        private Button buttonRemoveSource = null!;
        private CheckBox checkBoxSchedule = null!;
        private MaskedTextBox textBoxScheduleTime = null!;
        private Button buttonSave = null!;
    }
}
