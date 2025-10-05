namespace DemoSmartMonitoring
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            sidePanel = new Panel();
            buttonLogin = new Button();
            MainExitBtn = new Button();
            bottomPanel = new Panel();
            TopPanel = new Panel();
            TopMinimizeBtn = new Button();
            TopExitBtn = new Button();
            panelMain = new Panel();
            lbStatus = new Label();
            sidePanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            TopPanel.SuspendLayout();
            SuspendLayout();
            // 
            // sidePanel
            // 
            sidePanel.BackColor = Color.FromArgb(64, 64, 64);
            sidePanel.Controls.Add(buttonLogin);
            sidePanel.Controls.Add(MainExitBtn);
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Location = new Point(0, 0);
            sidePanel.Name = "sidePanel";
            sidePanel.Size = new Size(120, 800);
            sidePanel.TabIndex = 0;
            // 
            // buttonLogin
            // 
            buttonLogin.BackColor = Color.FromArgb(58, 58, 58);
            buttonLogin.BackgroundImageLayout = ImageLayout.Stretch;
            buttonLogin.Dock = DockStyle.Bottom;
            buttonLogin.FlatAppearance.BorderSize = 0;
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            buttonLogin.ForeColor = Color.White;
            buttonLogin.Image = Properties.Resources.AccountW;
            buttonLogin.ImageAlign = ContentAlignment.MiddleLeft;
            buttonLogin.Location = new Point(0, 700);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(120, 50);
            buttonLogin.TabIndex = 2;
            buttonLogin.Text = "Log in";
            buttonLogin.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonLogin.UseVisualStyleBackColor = false;
            // 
            // MainExitBtn
            // 
            MainExitBtn.BackColor = Color.FromArgb(58, 58, 58);
            MainExitBtn.BackgroundImageLayout = ImageLayout.Stretch;
            MainExitBtn.Dock = DockStyle.Bottom;
            MainExitBtn.FlatAppearance.BorderSize = 0;
            MainExitBtn.FlatStyle = FlatStyle.Flat;
            MainExitBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            MainExitBtn.ForeColor = Color.White;
            MainExitBtn.Image = (Image)resources.GetObject("MainExitBtn.Image");
            MainExitBtn.ImageAlign = ContentAlignment.MiddleLeft;
            MainExitBtn.Location = new Point(0, 750);
            MainExitBtn.Name = "MainExitBtn";
            MainExitBtn.Size = new Size(120, 50);
            MainExitBtn.TabIndex = 1;
            MainExitBtn.Text = "Exit";
            MainExitBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            MainExitBtn.UseVisualStyleBackColor = false;
            MainExitBtn.Click += MainExitBtn_Click;
            // 
            // bottomPanel
            // 
            bottomPanel.BackColor = Color.FromArgb(176, 176, 176);
            bottomPanel.Controls.Add(lbStatus);
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Location = new Point(120, 775);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(1280, 25);
            bottomPanel.TabIndex = 2;
            // 
            // TopPanel
            // 
            TopPanel.BackColor = Color.FromArgb(64, 64, 64);
            TopPanel.Controls.Add(TopMinimizeBtn);
            TopPanel.Controls.Add(TopExitBtn);
            TopPanel.Dock = DockStyle.Top;
            TopPanel.Location = new Point(120, 0);
            TopPanel.Name = "TopPanel";
            TopPanel.Size = new Size(1280, 25);
            TopPanel.TabIndex = 1;
            // 
            // TopMinimizeBtn
            // 
            TopMinimizeBtn.Dock = DockStyle.Right;
            TopMinimizeBtn.FlatAppearance.BorderSize = 0;
            TopMinimizeBtn.FlatStyle = FlatStyle.Flat;
            TopMinimizeBtn.Location = new Point(1130, 0);
            TopMinimizeBtn.Name = "TopMinimizeBtn";
            TopMinimizeBtn.Size = new Size(75, 25);
            TopMinimizeBtn.TabIndex = 1;
            TopMinimizeBtn.Text = "-";
            TopMinimizeBtn.UseVisualStyleBackColor = true;
            TopMinimizeBtn.Click += TopMinimizeBtn_Click;
            // 
            // TopExitBtn
            // 
            TopExitBtn.BackColor = Color.FromArgb(244, 67, 54);
            TopExitBtn.Dock = DockStyle.Right;
            TopExitBtn.FlatAppearance.BorderSize = 0;
            TopExitBtn.FlatStyle = FlatStyle.Flat;
            TopExitBtn.Location = new Point(1205, 0);
            TopExitBtn.Name = "TopExitBtn";
            TopExitBtn.Size = new Size(75, 25);
            TopExitBtn.TabIndex = 0;
            TopExitBtn.Text = "X";
            TopExitBtn.UseVisualStyleBackColor = false;
            TopExitBtn.Click += TopExitBtn_Click;
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.FromArgb(47, 47, 47);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(120, 25);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1280, 750);
            panelMain.TabIndex = 0;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            lbStatus.Location = new Point(21, 3);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(38, 15);
            lbStatus.TabIndex = 0;
            lbStatus.Text = "label1";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 800);
            Controls.Add(panelMain);
            Controls.Add(bottomPanel);
            Controls.Add(TopPanel);
            Controls.Add(sidePanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Main";
            Text = "Main System";
            sidePanel.ResumeLayout(false);
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            TopPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel sidePanel;
        private Button buttonLogin;
        private Button MainExitBtn;
        private Panel bottomPanel;
        private Panel TopPanel;
        private Button TopMinimizeBtn;
        private Button TopExitBtn;
        private Panel panelMain;
        private Label lbStatus;
    }
}
