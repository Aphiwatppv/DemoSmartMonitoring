namespace DemoSmartMonitoring
{
    public partial class Main : Form
    {
        private FormDragController dragger;

        // Host panel for swappable controls (sits above panelMain)
        private Panel panelHost;

        // Optional: cache UC instances so they aren’t recreated each time
        private readonly Dictionary<string, UserControl> _ucCache = new Dictionary<string, UserControl>();

        // Optional: single shared logger for the form (uncomment if you have LogSystem ready)
        // private readonly LogSystem.ILogSystem _logger = new LogSystem.LogSystem("MainLog", $"{DateTime.Now:ddMMyyHH}_Log");

        public Main()
        {
            // borderless window
            this.FormBorderStyle = FormBorderStyle.None;

            InitializeComponent();

            // draggable areas
            dragger = new FormDragController(components) { TargetForm = this };
            dragger.Attach(panelMain);
            dragger.Attach(sidePanel);
            dragger.Attach(TopPanel);

            // build a host panel for swapping user controls
            BuildHostPanel();

            // wire login button events
            buttonLogin.Click += ButtonLogin_Click;                 // single-click -> show login UC
            buttonLogin.MouseDoubleClick += ButtonLogin_MouseDoubleClick; // double-click -> back to main

            // start on panelMain
            ShowPanelMain();
        }

        private void BuildHostPanel()
        {
            panelHost = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false // hidden until we show a UC
            };

            // put host above panelMain in z-order
            this.Controls.Add(panelHost);
            panelHost.BringToFront();
        }

        // ------------------- Button Handlers -------------------
        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            ShowLoginControl();
        }

        private void ButtonLogin_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowPanelMain();
        }

        // ------------------- Swap Helpers -------------------
        /// <summary>
        /// Show a UserControl by key; create with factory if not already cached.
        /// Hides panelMain and displays the control inside panelHost.
        /// </summary>
        private void ShowControl(string key, Func<UserControl> factory)
        {
            // Hide main panel view
            panelMain.Visible = false;

            // Fetch from cache or create
            if (!_ucCache.TryGetValue(key, out var ctrl) || ctrl.IsDisposed)
            {
                ctrl = factory();
                ctrl.Dock = DockStyle.Fill;
                _ucCache[key] = ctrl;
            }

            // Mount control
            panelHost.SuspendLayout();
            panelHost.Controls.Clear();
            panelHost.Controls.Add(ctrl);
            panelHost.ResumeLayout();

            panelHost.Visible = true;
            ctrl.BringToFront();
            ctrl.Focus();
        }

        /// <summary>
        /// Show the default main panel (hide host).
        /// </summary>
        private void ShowPanelMain()
        {
            panelHost.Visible = false;
            panelMain.Visible = true;
            panelMain.BringToFront();
            panelMain.Focus();
        }

        // ------------------- Specific Views -------------------
        private void ShowLoginControl()
        {
            ShowControl("auth", () =>
            {
                var authUc = new DemoSmartMonitoring.Usercontroller.UserControlAuthen();
                // If your UserControl exposes success/cancel events, you can do:
                // authUc.AuthSucceeded += _ => ShowPanelMain();
                // authUc.AuthCancelled += () => ShowPanelMain();
                return authUc;
            });
        }

        // ------------------- Your existing handlers -------------------
        private void TopExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TopMinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void MainExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

