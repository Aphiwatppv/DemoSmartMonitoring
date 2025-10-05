using LogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSmartMonitoring
{
    public class AuthService
    {

 
        private readonly (string user, string pass)[] _users =
        {
            ("admin", "1234"),
            ("user",  "pass")
        };

        // Accessible anywhere in the application
        public static string CurrentUser { get; private set; }
        public static bool IsLogin { get; private set; }

        /// <summary>
        /// Attempts to log in using the provided credentials.
        /// </summary>
        public bool Login(string username, string password)
        {
          
            foreach (var u in _users)
            {
                if (u.user.Equals(username, StringComparison.OrdinalIgnoreCase) && u.pass == password)
                {
                    CurrentUser = u.user;
                    IsLogin = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        public void Logout()
        {
            CurrentUser = null;
            IsLogin = false;
        }

        /// <summary>
        /// Checks whether a user is authenticated.
        /// </summary>
        public bool IsAuthenticated()
        {
            return IsLogin && !string.IsNullOrEmpty(CurrentUser);
        }
    }
}
