using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoSmartMonitoring.Usercontroller
{
    public partial class UserControlAuthen : UserControl
    {
        private LogSystem.ILogSystem _logSystem;

        public UserControlAuthen()
        {
            InitializeComponent();

            // Initialize logger ONCE (not every click)
            try
            {
                _logSystem = new LogSystem.LogSystem("MainLog", "{date}Log");
                _logSystem.Info("UserControlAuthen initialized.");
            }
            catch (Exception ex)
            {
                // Fallback: avoid crashing UI if logger fails
                MessageBox.Show($"Logger initialization failed: {ex.Message}", "Logging",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateUiState();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // ---- LOGOUT FLOW ----
                if (AuthService.IsLogin)
                {
                    _logSystem?.Info($"Logout requested by '{AuthService.CurrentUser}'.");
                    var confirm = MessageBox.Show("Do you want to log out?", "Confirm Logout",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.Yes)
                    {
                        var auth = new AuthService();
                        auth.Logout();

                        _logSystem?.Info("Logout successful.");

                        MessageBox.Show("You have been logged out successfully.", "Logout",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        UpdateUiState();
                    }
                    else
                    {
                        _logSystem?.Info("Logout cancelled by user.");
                    }

                    return;
                }

                // ---- LOGIN FLOW ----
                var username = txtUsername.Text.Trim();
                var password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logSystem?.Warning("Login attempt with empty username or password.");
                    MessageBox.Show("Please enter both username and password.", "Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _logSystem?.Info($"Login attempt for user '{username}' from '{Environment.MachineName}'.");

                var authSvc = new AuthService();
                if (authSvc.Login(username, password))
                {
                    _logSystem?.Info($"Login successful for '{AuthService.CurrentUser}'.");
                    MessageBox.Show($"Welcome {AuthService.CurrentUser}!", "Login Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateUiState();
                }
                else
                {
                    _logSystem?.Error($"Login failed for '{username}'. Invalid credentials.");
                    MessageBox.Show("Invalid username or password.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                }
                else
                {
                    txtUsername.ReadOnly = false;
                    txtPassword.ReadOnly = false;
                    txtUsername.Clear();
                    txtPassword.Clear();
                    BtnLogin.Text = "Login";
                    lblStatus.Text = "Please login.";
                }

                _logSystem?.Warning($"UI state updated. IsLogin={AuthService.IsLogin}, User='{AuthService.CurrentUser}'.");
            }
            catch (Exception ex)
            {
                _logSystem?.Error($"UpdateUiState error: {ex}");
            }
        }
    }
}
