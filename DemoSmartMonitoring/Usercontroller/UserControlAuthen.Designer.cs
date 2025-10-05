namespace DemoSmartMonitoring.Usercontroller
{
    partial class UserControlAuthen
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            BtnLogin = new Button();
            label1 = new Label();
            label2 = new Label();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(451, 304);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(463, 23);
            txtUsername.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(451, 353);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(463, 23);
            txtPassword.TabIndex = 1;
            // 
            // BtnLogin
            // 
            BtnLogin.BackColor = Color.FromArgb(33, 150, 243);
            BtnLogin.FlatAppearance.BorderSize = 0;
            BtnLogin.FlatStyle = FlatStyle.Flat;
            BtnLogin.Font = new Font("Segoe UI", 10F);
            BtnLogin.ForeColor = Color.FromArgb(224, 224, 224);
            BtnLogin.Location = new Point(451, 451);
            BtnLogin.Name = "BtnLogin";
            BtnLogin.Size = new Size(124, 48);
            BtnLogin.TabIndex = 2;
            BtnLogin.Text = "Login";
            BtnLogin.UseVisualStyleBackColor = false;
            BtnLogin.Click += BtnLogin_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.FromArgb(241, 241, 241);
            label1.Location = new Point(376, 307);
            label1.Name = "label1";
            label1.Size = new Size(69, 15);
            label1.TabIndex = 3;
            label1.Text = "Username : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.FromArgb(241, 241, 241);
            label2.Location = new Point(376, 353);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 4;
            label2.Text = "Password : ";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.FromArgb(241, 241, 241);
            lblStatus.Location = new Point(451, 400);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(45, 15);
            lblStatus.TabIndex = 5;
            lblStatus.Text = "Status :";
            // 
            // UserControlAuthen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(47, 47, 47);
            Controls.Add(lblStatus);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(BtnLogin);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Name = "UserControlAuthen";
            Size = new Size(1247, 810);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button BtnLogin;
        private Label label1;
        private Label label2;
        private Label lblStatus;
    }
}
