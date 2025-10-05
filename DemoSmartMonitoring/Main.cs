using Timer = System.Windows.Forms.Timer;

namespace DemoSmartMonitoring
{
    public partial class Main : Form
    {
        private FormDragController dragger;
        private Panel panelHost;
        private readonly Dictionary<string, UserControl> _ucCache = new();
        private string _currentKey;

        private readonly System.Windows.Forms.Timer _clickTimer;

        // NEW: sidebar highlight
        private Panel _navHighlight;
        private readonly Timer _navTimer = new Timer { Interval = 10 };
        private int _targetY;

        public Main()
        {
            
            InitializeComponent();

            dragger = new FormDragController(components) { TargetForm = this };
            dragger.Attach(panelMain);
            dragger.Attach(sidePanel);
            dragger.Attach(TopPanel);

            BuildHostPanel();
            BuildNavHighlight(); // NEW

            // highlight animation timer
            _navTimer.Tick += (s, e) =>
            {
                const int step = 8;
                int dy = _targetY - _navHighlight.Top;
                if (Math.Abs(dy) <= step)
                {
                    _navHighlight.Top = _targetY;
                    _navTimer.Stop();
                }
                else
                {
                    _navHighlight.Top += Math.Sign(dy) * step;
                }
            };

            _clickTimer = new Timer { Interval = SystemInformation.DoubleClickTime };
            _clickTimer.Tick += (s, e) =>
            {
                _clickTimer.Stop();
                ShowOrToggle("auth", () =>
                {
                    var uc = new DemoSmartMonitoring.Usercontroller.UserControlAuthen();

                    uc.AuthSucceeded += user =>
                    {
                        lbStatus.Text = $"Login as: {user}";
                        ApplyAuthGate();          // enables sidebar
                        ShowPanelMain();          // go back to main content
                    };

                    uc.LoggedOut += () =>
                    {
                        ApplyAuthGate();          // disables sidebar + resets status
                    };

                    return uc;
                });
            };

            buttonLogin.Click += ButtonLogin_Click;
            buttonLogin.MouseDown += ButtonLogin_MouseDown;

            ShowPanelMain();
        }

        private void BuildHostPanel()
        {
            panelHost = new Panel { Dock = DockStyle.Fill, Visible = false };
            this.Controls.Add(panelHost);
            panelHost.BringToFront();
            BuildNavHighlight();
            // animate highlight
            _navTimer.Tick += (s, e) =>
            {
                const int step = 8;
                int dy = _targetY - _navHighlight.Top;
                if (Math.Abs(dy) <= step)
                {
                    _navHighlight.Top = _targetY;
                    _navTimer.Stop();
                }
                else
                {
                    _navHighlight.Top += Math.Sign(dy) * step;
                }
            };

            // lock UI initially
            ApplyAuthGate();
        }

        // NEW
        private void BuildNavHighlight()
        {
            _navHighlight = new Panel
            {
                Width = 3,
                Height = buttonLogin.Height,
                Left = 0,
                Top = buttonLogin.Top,
                BackColor = Color.FromArgb(28, 169, 201),
                Visible = true
            };
            sidePanel.Controls.Add(_navHighlight);
            _navHighlight.BringToFront();
        }

        private void SetSidebarEnabled(bool enabled)
        {
            foreach (Control c in sidePanel.Controls)
            {
                if (c is Button b && !ReferenceEquals(b, buttonLogin))
                    b.Enabled = enabled;
            }
        }

        // NEW
        private void MoveNavHighlight(Control btn)
        {
            if (btn == null) return;
            _navHighlight.Height = btn.Height;
            _targetY = btn.Top;
            if (!_navTimer.Enabled) _navTimer.Start();
        }
        private void ApplyAuthGate()
        {
            if (AuthService.IsLogin)
            {
                lbStatus.Text = $"Login as: {AuthService.CurrentUser}";
                SetSidebarEnabled(true);
            }
            else
            {
                lbStatus.Text = "Please login.";
                SetSidebarEnabled(false);
                MoveNavHighlight(buttonLogin);
                ShowPanelMain();
            }
        }
        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            _clickTimer.Stop();
            _clickTimer.Start();
            MoveNavHighlight(buttonLogin); // NEW
        }

        private void ButtonLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks >= 2)
            {
                _clickTimer.Stop();
                ShowPanelMain();
                MoveNavHighlight(buttonLogin); // optional keep the highlight
            }
        }

        private void ShowControl(string key, Func<UserControl> factory)
        {
            panelMain.Visible = false;

            if (!_ucCache.TryGetValue(key, out var ctrl) || ctrl.IsDisposed)
            {
                ctrl = factory();
                ctrl.Dock = DockStyle.Fill;
                _ucCache[key] = ctrl;
            }

            panelHost.SuspendLayout();
            panelHost.Controls.Clear();
            panelHost.Controls.Add(ctrl);
            panelHost.ResumeLayout();

            panelHost.Visible = true;
            ctrl.BringToFront();
            ctrl.Focus();
            _currentKey = key;
        }

        private void ShowOrToggle(string key, Func<UserControl> factory)
        {
            if (panelHost.Visible && string.Equals(_currentKey, key, StringComparison.Ordinal))
                ShowPanelMain();
            else
                ShowControl(key, factory);
        }

        private void ShowPanelMain()
        {
            panelHost.Visible = false;
            panelHost.Controls.Clear();
            panelMain.Visible = true;
            panelMain.BringToFront();
            panelMain.Focus();
            _currentKey = null;
        }

        private void ShowLoginControl()
        {
            ShowOrToggle("auth", () =>
            {
                var uc = new DemoSmartMonitoring.Usercontroller.UserControlAuthen();

                uc.AuthSucceeded += user =>
                {
                    lbStatus.Text = $"Login as: {user}";
                    ApplyAuthGate();          // enables sidebar
                    ShowPanelMain();          // go back to main content
                };

                uc.LoggedOut += () =>
                {
                    ApplyAuthGate();          // disables sidebar + resets status
                };

                return uc;
            });
        }

        private void TopExitBtn_Click(object sender, EventArgs e) => Application.Exit();
        private void TopMinimizeBtn_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void MainExitBtn_Click(object sender, EventArgs e) => Application.Exit();
    }

}

