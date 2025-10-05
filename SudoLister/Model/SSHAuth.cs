using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoLister.Model
{
    public sealed class SSHAuth
    {
        public string Hostname { get; init; } = "";
        public int Port { get; init; } = 22;
        public string Username { get; init; } = "";   // your normal login user
        public string? Password { get; init; } = null;
        public string? KeyFile { get; init; } = null; // OpenSSH private key (not .ppk)
        public string? KeyPassphrase { get; init; } = null;
        public bool SudoRequireTty { get; init; } = false; // true if sudoers demands TTY
    }
}
