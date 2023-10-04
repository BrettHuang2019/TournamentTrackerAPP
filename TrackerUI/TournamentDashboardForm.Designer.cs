namespace TrackerUI
{
    partial class TournamentDashboardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            headerLabel = new Label();
            loadExistedTournamentDropdown = new ComboBox();
            loadExistedTournamentLabel = new Label();
            loadTournamentButton = new Button();
            createTournamentButton = new Button();
            SuspendLayout();
            // 
            // headerLabel
            // 
            headerLabel.AutoSize = true;
            headerLabel.Font = new Font("Segoe UI", 27.75F, FontStyle.Regular, GraphicsUnit.Point);
            headerLabel.ForeColor = SystemColors.MenuHighlight;
            headerLabel.Location = new Point(102, 9);
            headerLabel.Name = "headerLabel";
            headerLabel.Size = new Size(408, 50);
            headerLabel.TabIndex = 15;
            headerLabel.Text = "Tournament Dashboard";
            // 
            // loadExistedTournamentDropdown
            // 
            loadExistedTournamentDropdown.FormattingEnabled = true;
            loadExistedTournamentDropdown.Location = new Point(168, 175);
            loadExistedTournamentDropdown.Name = "loadExistedTournamentDropdown";
            loadExistedTournamentDropdown.Size = new Size(276, 38);
            loadExistedTournamentDropdown.TabIndex = 20;
            loadExistedTournamentDropdown.SelectedIndexChanged += loadExistedTournamentDropdown_SelectedIndexChanged;
            // 
            // loadExistedTournamentLabel
            // 
            loadExistedTournamentLabel.AutoSize = true;
            loadExistedTournamentLabel.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            loadExistedTournamentLabel.ForeColor = SystemColors.MenuHighlight;
            loadExistedTournamentLabel.Location = new Point(149, 120);
            loadExistedTournamentLabel.Name = "loadExistedTournamentLabel";
            loadExistedTournamentLabel.Size = new Size(314, 37);
            loadExistedTournamentLabel.TabIndex = 19;
            loadExistedTournamentLabel.Text = "Load Existed Tournament";
            // 
            // loadTournamentButton
            // 
            loadTournamentButton.FlatAppearance.BorderColor = Color.Silver;
            loadTournamentButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(102, 102, 102);
            loadTournamentButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(242, 242, 242);
            loadTournamentButton.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            loadTournamentButton.ForeColor = SystemColors.MenuHighlight;
            loadTournamentButton.Location = new Point(172, 231);
            loadTournamentButton.Name = "loadTournamentButton";
            loadTournamentButton.Size = new Size(269, 41);
            loadTournamentButton.TabIndex = 25;
            loadTournamentButton.Text = "Load Tournament";
            loadTournamentButton.UseVisualStyleBackColor = true;
            loadTournamentButton.Click += loadTournamentButton_Click;
            // 
            // createTournamentButton
            // 
            createTournamentButton.FlatAppearance.BorderColor = Color.Silver;
            createTournamentButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(102, 102, 102);
            createTournamentButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(242, 242, 242);
            createTournamentButton.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            createTournamentButton.ForeColor = SystemColors.MenuHighlight;
            createTournamentButton.Location = new Point(172, 290);
            createTournamentButton.Name = "createTournamentButton";
            createTournamentButton.Size = new Size(269, 41);
            createTournamentButton.TabIndex = 26;
            createTournamentButton.Text = "Create Tournament";
            createTournamentButton.UseVisualStyleBackColor = true;
            createTournamentButton.Click += createTournamentButton_Click;
            // 
            // TournamentDashboardForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(613, 384);
            Controls.Add(createTournamentButton);
            Controls.Add(loadTournamentButton);
            Controls.Add(loadExistedTournamentDropdown);
            Controls.Add(loadExistedTournamentLabel);
            Controls.Add(headerLabel);
            Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(5, 6, 5, 6);
            Name = "TournamentDashboardForm";
            Text = "Tournament Dashboard";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label headerLabel;
        private ComboBox loadExistedTournamentDropdown;
        private Label loadExistedTournamentLabel;
        private Button loadTournamentButton;
        private Button createTournamentButton;
    }
}