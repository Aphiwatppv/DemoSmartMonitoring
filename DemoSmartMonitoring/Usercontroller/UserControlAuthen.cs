using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DemoSmartMonitoring.Usercontroller
{
    public partial class UserControlAuthen : UserControl
    {
        private LogSystem.ILogSystem _logSystem;
        public event Action<string> AuthSucceeded;
        public event Action LoggedOut;
        // Theme
        private readonly Color BgCanvas = Color.FromArgb(47, 47, 47);
        private readonly Color BgCard = Color.FromArgb(58, 58, 58);
        private readonly Color Accent = ColorTranslator.FromHtml("#1CA9C9");
        private readonly Color TextPri = Color.FromArgb(240, 240, 240);
        private readonly Color TextSec = Color.FromArgb(170, 170, 170);
        private readonly Color Border = Color.FromArgb(80, 80, 80);

        // UI
        private Panel pnlRoot, pnlCard;
        private TableLayoutPanel tlp;
        private Label lblTitle, lblSubtitle, lblStatus, lblUser, lblPass;
        private TextBox txtUsername, txtPassword;
        private Panel underlineUser, underlinePass;
        private Button btnTogglePwd, BtnLogin;
        private ToolTip tip;

        public UserControlAuthen()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            BuildUi();
            WireInteractions();

            try
            {
                // If your LogSystem expands {date} internally, keep this.
                // Otherwise you can use $"{DateTime.Now:ddMMyyHH}_Log"
                _logSystem = new LogSystem.LogSystem("MainLog", "Log");
                _logSystem?.Info("UserControlAuthen initialized (dark modern).");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logger initialization failed: {ex.Message}", "Logging",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateUiState();
        }

        private void BuildUi()
        {
            Controls.Clear();

            // === Theme ===
            var BgCanvas = Color.FromArgb(47, 47, 47);
            var BgCard = Color.FromArgb(58, 58, 58);
            var Accent = ColorTranslator.FromHtml("#1CA9C9");
            var TextPri = Color.FromArgb(240, 240, 240);
            var TextSec = Color.FromArgb(170, 170, 170);
            var Border = Color.FromArgb(80, 80, 80);

            // Save to fields if you reference them elsewhere
            this.BackColor = BgCanvas;

            // Root
            pnlRoot = new Panel { Dock = DockStyle.Fill, BackColor = BgCanvas };
            Controls.Add(pnlRoot);

            // Card (fixed size, centered)
            pnlCard = new Panel
            {
                BackColor = BgCard,
                Size = new Size(460, 360),
                BorderStyle = BorderStyle.None
            };
            pnlRoot.Controls.Add(pnlCard);
            Resize += (s, e) => CenterCard();
            CenterCard();

            // Table
            tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = BgCard,
                ColumnCount = 2,
                Padding = new Padding(24, 18, 24, 18),
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // main column
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // toggle button column
            pnlCard.Controls.Add(tlp);

            int row = 0;

            // Title
            lblTitle = new Label
            {
                Text = "Sign in",
                AutoSize = true,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = TextPri,
                Margin = new Padding(0, 0, 0, 2)
            };
            tlp.Controls.Add(lblTitle, 0, row);
            tlp.SetColumnSpan(lblTitle, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            row++;

            // Subtitle
            lblSubtitle = new Label
            {
                Text = "Enter your credentials to continue",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                ForeColor = TextSec,
                Margin = new Padding(0, 0, 0, 12)
            };
            tlp.Controls.Add(lblSubtitle, 0, row);
            tlp.SetColumnSpan(lblSubtitle, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            row++;

            // Username label
            lblUser = new Label
            {
                Text = "Username",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = TextSec,
                Margin = new Padding(0, 4, 0, 2)
            };
            tlp.Controls.Add(lblUser, 0, row);
            tlp.SetColumnSpan(lblUser, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            row++;

            // Username input
            txtUsername = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                ForeColor = TextPri,
                BackColor = Color.FromArgb(58, 58, 58),
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 0)
            };
            tlp.Controls.Add(txtUsername, 0, row);
            tlp.SetColumnSpan(txtUsername, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            row++;

            // Username underline
            underlineUser = new Panel { BackColor = Border, Height = 1, Dock = DockStyle.Top, Margin = new Padding(0, 2, 0, 0) };
            tlp.Controls.Add(underlineUser, 0, row);
            tlp.SetColumnSpan(underlineUser, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 1));
            row++;

            // Spacer
            tlp.Controls.Add(new Panel { Height = 1, Dock = DockStyle.Top }, 0, row);
            tlp.SetColumnSpan(tlp.GetControlFromPosition(0, row), 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 8));
            row++;

            // Password label
            lblPass = new Label
            {
                Text = "Password",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = TextSec,
                Margin = new Padding(0, 4, 0, 2)
            };
            tlp.Controls.Add(lblPass, 0, row);
            tlp.SetColumnSpan(lblPass, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            row++;

            // Password row (TEXTBOX in col 0, TOGGLE in col 1 — SAME ROW)
            txtPassword = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11F),
                ForeColor = TextPri,
                BackColor = Color.FromArgb(58, 58, 58),
                UseSystemPasswordChar = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnTogglePwd = new Button
            {
                Text = "Show",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(58, 58, 58),
                ForeColor = Accent,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(8, 0, 0, 0),
                TabStop = false
            };
            btnTogglePwd.FlatAppearance.BorderSize = 0;

            // Add both to the SAME row, different columns
            tlp.Controls.Add(txtPassword, 0, row);
            tlp.Controls.Add(btnTogglePwd, 1, row);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            row++;

            // Password underline
            underlinePass = new Panel { BackColor = Border, Height = 1, Dock = DockStyle.Top, Margin = new Padding(0, 2, 0, 0) };
            tlp.Controls.Add(underlinePass, 0, row);
            tlp.SetColumnSpan(underlinePass, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 1));
            row++;

            // Spacer
            tlp.Controls.Add(new Panel { Height = 1, Dock = DockStyle.Top }, 0, row);
            tlp.SetColumnSpan(tlp.GetControlFromPosition(0, row), 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
            row++;

            // Login button
            BtnLogin = new Button
            {
                Text = "Login",
                FlatStyle = FlatStyle.Flat,
                BackColor = Accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Height = 40,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8)
            };
            BtnLogin.FlatAppearance.BorderSize = 0;
            BtnLogin.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(Accent, .05f);
            BtnLogin.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(Accent, .15f);
            tlp.Controls.Add(BtnLogin, 0, row);
            tlp.SetColumnSpan(BtnLogin, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            row++;

            // Status
            lblStatus = new Label
            {
                Text = "Please login.",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextSec
            };
            tlp.Controls.Add(lblStatus, 0, row);
            tlp.SetColumnSpan(lblStatus, 2);
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            row++;

            // ToolTip & placeholders
            tip = new ToolTip();
            tip.SetToolTip(btnTogglePwd, "Show / hide password");
            SetCueBanner(txtUsername, "e.g., admin");
            SetCueBanner(txtPassword, "Your password");
        }





        private TextBox MakeInput() => new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 11F),
            ForeColor = TextPri,
            BackColor = BgCard,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 0)
        };

        private Panel MakeUnderline() => new Panel
        {
            BackColor = Border,
            Height = 1,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 2, 0, 0)
        };

        private void CenterCard()
        {
            if (pnlCard == null) return;
            var x = Math.Max(0, (Width - pnlCard.Width) / 2);
            var y = Math.Max(0, (Height - pnlCard.Height) / 2);
            pnlCard.Location = new Point(x, y);
            Invalidate(new Rectangle(pnlCard.Location, pnlCard.Size));
        }

        private void WireInteractions()
        {
            // Focus glow
            txtUsername.Enter += (s, e) => underlineUser.BackColor = Accent;
            txtUsername.Leave += (s, e) => underlineUser.BackColor = Border;
            txtPassword.Enter += (s, e) => underlinePass.BackColor = Accent;
            txtPassword.Leave += (s, e) => underlinePass.BackColor = Border;

            // Enter to login
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin.PerformClick(); };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin.PerformClick(); };

            // Toggle password visibility
            btnTogglePwd.Click += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
                btnTogglePwd.Text = txtPassword.UseSystemPasswordChar ? "Show" : "Hide";
            };

            // Make parent form accept Enter as login if free
            HandleCreated += (s, e) =>
            {
                var form = FindForm();
                if (form != null && form.AcceptButton == null)
                    form.AcceptButton = BtnLogin;
            };

            // Login/Logout
            BtnLogin.Click += BtnLogin_Click;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (AuthService.IsLogin)
                {
                    _logSystem?.Info($"Logout requested by '{AuthService.CurrentUser}'.");
                    var confirm = MessageBox.Show("Do you want to log out?", "Confirm Logout",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        new AuthService().Logout();
                        _logSystem?.Info("Logout successful.");
                        UpdateUiState();
                        LoggedOut?.Invoke();
                    }
                    else
                    {
                        _logSystem?.Info("Logout cancelled by user.");
                    }
                    return;
                }

                var username = txtUsername.Text.Trim();
                var password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logSystem?.Warning("Login attempt with empty username or password.");
                    lblStatus.ForeColor = Color.Orange;
                    lblStatus.Text = "Please enter both username and password.";
                    Shake(pnlCard);
                    return;
                }

                _logSystem?.Info($"Login attempt for user '{username}' from '{Environment.MachineName}'.");

                var authSvc = new AuthService();
                if (authSvc.Login(username, password))
                {
                    _logSystem?.Info($"Login successful for '{AuthService.CurrentUser}'.");
                    lblStatus.ForeColor = Color.LightGreen;
                    lblStatus.Text = $"Welcome {AuthService.CurrentUser}";
                    UpdateUiState();
                    // notify host form
                    AuthSucceeded?.Invoke(AuthService.CurrentUser);
                    return;

                }
                else
                {
                    _logSystem?.Error($"Login failed for '{username}'. Invalid credentials.");
                    lblStatus.ForeColor = Color.LightCoral;
                    lblStatus.Text = "Invalid username or password.";
                    Shake(pnlCard);
                }
            }
            catch (Exception ex)
            {
                _logSystem?.Error($"Unhandled exception in BtnLogin_Click: {ex}");
                MessageBox.Show("An unexpected error occurred. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUiState()
        {
            try
            {
                if (AuthService.IsLogin)
                {
                    txtUsername.ReadOnly = true;
                    txtPassword.ReadOnly = true;
                    BtnLogin.Text = "Log out";
                    lblStatus.Text = $"Logged in as: {AuthService.CurrentUser}";
                    lblStatus.ForeColor = TextSec;
                }
                else
                {
                    txtUsername.ReadOnly = false;
                    txtPassword.ReadOnly = false;
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtUsername.Focus();
                    BtnLogin.Text = "Login";
                    lblStatus.Text = "Please login.";
                    lblStatus.ForeColor = TextSec;
                }

                _logSystem?.Warning($"UI updated. IsLogin={AuthService.IsLogin}, User='{AuthService.CurrentUser}'.");
            }
            catch (Exception ex)
            {
                _logSystem?.Error($"UpdateUiState error: {ex}");
            }
        }

        // Subtle shake animation on invalid input
        private async void Shake(Control c)
        {
            var baseX = c.Left;
            for (int i = 0; i < 2; i++)
            {
                c.Left = baseX - 6; await System.Threading.Tasks.Task.Delay(20);
                c.Left = baseX + 6; await System.Threading.Tasks.Task.Delay(20);
            }
            c.Left = baseX;
        }

        // Cue banner placeholders
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;
        private void SetCueBanner(TextBox tb, string text)
        {
            try { SendMessage(tb.Handle, EM_SETCUEBANNER, 1, text); } catch { /* ignore */ }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 1px flat border around card
            using var pen = new Pen(Border, 1);
            var r = pnlCard.Bounds;
            r.Width -= 1; r.Height -= 1;
            e.Graphics.DrawRectangle(pen, r);
        }
    }
}
