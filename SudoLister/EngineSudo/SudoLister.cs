using LogSystem;
using Renci.SshNet;
using Renci.SshNet.Common;
using SudoLister.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SudoLister.EngineSudo
{
    public class SudoLister
    {
        private readonly SSHAuth _auth;
        private readonly string _sudoUser;
        private readonly LogSystem.ILogSystem _logSystem;

        public SudoLister(SSHAuth auth, string sudoUser)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _sudoUser = string.IsNullOrWhiteSpace(sudoUser) ? throw new ArgumentException("sudoUser required") : sudoUser;
            _logSystem = new LogSystem.LogSystem("Log", "Investigation");
        }

        // ======================================================
        // 1) SUDO-ONLY LISTING / SEARCH / SUMMARY  (instance API)
        // ======================================================

        public List<FileEntry> List(string basePath, bool recursive, bool includeFiles = true, bool includeDirs = false)
            => ExecFind(basePath, recursive, includeFiles, includeDirs);

        public List<FileEntry> FindByName(string basePath, string filename)
            => ExecFind(basePath, recursive: true, includeFiles: true, includeDirs: false)
               .Where(f => string.Equals(f.Name, filename, StringComparison.OrdinalIgnoreCase))
               .ToList();

        public List<FileEntry> FindContaining(string basePath, string substring)
            => ExecFind(basePath, recursive: true, includeFiles: true, includeDirs: false)
               .Where(f => f.Name.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0)
               .ToList();

        public Dictionary<string, List<FileEntry>> SummarizeByName(string basePath, string filename)
            => GroupByDirectory(FindByName(basePath, filename));

        public Dictionary<string, List<FileEntry>> SummarizeContaining(string basePath, string substring)
            => GroupByDirectory(FindContaining(basePath, substring));

        // ==========================
        // 2) METRICS (instance API)
        // ==========================

        public TransferRateResult MeasureTransferRate(string basePath, TimeSpan window, bool recursive = true, string? nameContains = null)
        {
            try
            {
                if (window <= TimeSpan.Zero) throw new ArgumentException("window must be > 0", nameof(window));
                var now = DateTime.Now;
                var start = now - window;

                _logSystem.Info($"MeasureTransferRate start: sudoUser={_sudoUser}, path={basePath}, window={window}, recursive={recursive}, filter={nameContains ?? "-"}");

                var rows = ExecFind(basePath, recursive, includeFiles: true, includeDirs: false);

                IEnumerable<FileEntry> filtered = rows.Where(f => f.MTime.HasValue && f.MTime.Value >= start && f.MTime.Value <= now);
                if (!string.IsNullOrWhiteSpace(nameContains))
                    filtered = filtered.Where(f => f.Name.Contains(nameContains, StringComparison.OrdinalIgnoreCase));

                int count = filtered.Count();
                var result = new TransferRateResult
                {
                    Files = count,
                    Window = window,
                    WindowStart = start,
                    WindowEnd = now,
                    PerMinute = count / window.TotalMinutes,
                    PerHour = count / window.TotalHours
                };

                _logSystem.Info($"MeasureTransferRate done: files={result.Files}, perMin={result.PerMinute:F2}, perHour={result.PerHour:F2}");
                return result;
            }
            catch (Exception ex)
            {
                _logSystem.Error($"MeasureTransferRate error: {ex.Message}");
                throw;
            }
        }

        public TransferRateResult MeasureTransferRateLastMinutes(string basePath, int minutes, bool recursive = true, string? nameContains = null)
            => MeasureTransferRate(basePath, TimeSpan.FromMinutes(minutes), recursive, nameContains);

        public TransferRateResult MeasureTransferRateLastHours(string basePath, double hours, bool recursive = true, string? nameContains = null)
            => MeasureTransferRate(basePath, TimeSpan.FromHours(hours), recursive, nameContains);

        public QueueSizeResult GetQueueSize(string folderPath, bool recursive = false, string? nameContains = null)
        {
            try
            {
                _logSystem.Info($"GetQueueSize start: sudoUser={_sudoUser}, path={folderPath}, recursive={recursive}, filter={nameContains ?? "-"}");

                var rows = ExecFind(folderPath, recursive, includeFiles: true, includeDirs: false);

                IEnumerable<FileEntry> filtered = rows;
                if (!string.IsNullOrWhiteSpace(nameContains))
                    filtered = filtered.Where(f => f.Name.Contains(nameContains, StringComparison.OrdinalIgnoreCase));

                var list = filtered.ToList();
                var result = new QueueSizeResult { Pending = list.Count, Items = list };

                _logSystem.Info($"GetQueueSize done: pending={result.Pending}");
                if (result.Pending == 0) _logSystem.Warning("GetQueueSize: no pending files found.");
                return result;
            }
            catch (Exception ex)
            {
                _logSystem.Error($"GetQueueSize error: {ex.Message}");
                throw;
            }
        }


        // ==========================================================
        // 4) COPY: LOCAL → REMOTE  (SFTP to /tmp + sudo mv + chmod/chown)
        // ==========================================================
        public void CopyFileToRemote(string localPath, string remotePath, string? chmod = null, string? chown = null)
        {
            if (!File.Exists(localPath)) throw new FileNotFoundException("Local file not found", localPath);

            string q(string s) => "'" + s.Replace("'", "'\"'\"'") + "'";
            var tempName = $"upload_{Guid.NewGuid():N}";
            var tempRemote = $"/tmp/{tempName}";

            try
            {
                _logSystem.Info($"CopyFileToRemote start: sudoUser={_sudoUser}, local={localPath}, remote={remotePath}, chmod={chmod ?? "-"}, chown={chown ?? "-"}");

                // 1) SFTP upload to temp
                using (var sftp = CreateSftp(_auth))
                {
                    sftp.Connect();
                    using var lf = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    sftp.UploadFile(lf, tempRemote);
                    sftp.Disconnect();
                }

                // 2) sudo move to final + chmod/chown if requested
                var destDir = $"$(dirname {q(remotePath)})";
                string script = $"mkdir -p {destDir} && mv {q(tempRemote)} {q(remotePath)}";
                if (!string.IsNullOrWhiteSpace(chmod)) script += $" && chmod {chmod} {q(remotePath)}";
                if (!string.IsNullOrWhiteSpace(chown)) script += $" && chown {chown} {q(remotePath)}";
                var fullCmd = $"sudo -iu {q(_sudoUser)} bash -lc {q(script)}";

                using var ssh = CreateSsh(_auth);
                ssh.Connect();
                var cmd = ssh.CreateCommand(fullCmd);
                var res = cmd.Execute();
                var exit = cmd.ExitStatus;
                ssh.Disconnect();

                if (exit != 0)
                {
                    _logSystem.Error($"CopyFileToRemote failed (exit {exit}). Error: {cmd.Error}");
                    throw new Exception($"sudo move failed (exit {exit}). Error: {cmd.Error}");
                }

                _logSystem.Info($"CopyFileToRemote done: {localPath} -> {remotePath}");
            }
            catch (Exception ex)
            {
                _logSystem.Error($"CopyFileToRemote error ({localPath} -> {remotePath}): {ex.Message}");
                // Best-effort cleanup of temp on remote is possible via another sudo command if needed.
                throw;
            }
        }

        // ==========================
        // 5) INTERNAL HELPERS
        // ==========================
        private List<FileEntry> ExecFind(string basePath, bool recursive, bool includeFiles, bool includeDirs)
        {
            string q(string s) => "'" + s.Replace("'", "'\"'\"'") + "'";
            var depthFlag = recursive ? "-mindepth 1" : "-maxdepth 1";
            var findCmd = $"find {q(basePath)} {depthFlag} -printf '{{\"t\":\"%y\",\"s\":%s,\"m\":%T@,\"p\":\"%p\"}}\\n'";
            var fullCmd = $"sudo -iu {q(_sudoUser)} bash -lc {q(findCmd)}";

            try
            {
                _logSystem.Info($"ExecFind start: sudoUser={_sudoUser}, path={basePath}, recursive={recursive}, files={includeFiles}, dirs={includeDirs}");
                string output = Run(_auth, fullCmd);

                var list = new List<FileEntry>();
                using var sr = new StringReader(output);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    FindRec? rec = null;
                    try { rec = JsonSerializer.Deserialize<FindRec>(line); } catch { /* skip */ }
                    if (rec == null) continue;

                    bool isDir = rec.t == "d";
                    if ((isDir && includeDirs) || (!isDir && includeFiles))
                    {
                        DateTime? mtime = null;
                        if (double.TryParse(rec.m, out var epoch))
                            mtime = DateTimeOffset.FromUnixTimeSeconds((long)epoch).LocalDateTime;

                        var name = rec.p.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";
                        list.Add(new FileEntry { Name = name, Path = rec.p, IsDir = isDir, Size = rec.s, MTime = mtime });
                    }
                }

                list = list.OrderBy(r => r.Path).ThenBy(r => r.Name).ToList();
                _logSystem.Info($"ExecFind done: {list.Count} entries");
                if (list.Count == 0) _logSystem.Warning("ExecFind: no entries found.");
                return list;
            }
            catch (Exception ex)
            {
                _logSystem.Error($"ExecFind error ({basePath}): {ex.Message}");
                throw;
            }
        }

        private static Dictionary<string, List<FileEntry>> GroupByDirectory(IEnumerable<FileEntry> rows)
            => rows.GroupBy(f => DirName(f.Path))
                   .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Name).ToList());

        private static string DirName(string path)
        {
            var p = path.Replace('\\', '/');
            var i = p.LastIndexOf('/');
            return i <= 0 ? "/" : p[..i];
        }

        private static SshClient CreateSsh(SSHAuth auth)
        {
            if (!string.IsNullOrWhiteSpace(auth.KeyFile))
            {
                var key = string.IsNullOrEmpty(auth.KeyPassphrase)
                    ? new PrivateKeyFile(auth.KeyFile)
                    : new PrivateKeyFile(auth.KeyFile, auth.KeyPassphrase);

                var conn = new ConnectionInfo(
                    auth.Hostname, auth.Port, auth.Username,
                    new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod(auth.Username, key) }
                );
                return new SshClient(conn);
            }
            return new SshClient(auth.Hostname, auth.Port, auth.Username, auth.Password);
        }

        private static SftpClient CreateSftp(SSHAuth auth)
        {
            if (!string.IsNullOrWhiteSpace(auth.KeyFile))
            {
                var key = string.IsNullOrEmpty(auth.KeyPassphrase)
                    ? new PrivateKeyFile(auth.KeyFile)
                    : new PrivateKeyFile(auth.KeyFile, auth.KeyPassphrase);

                var conn = new ConnectionInfo(
                    auth.Hostname, auth.Port, auth.Username,
                    new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod(auth.Username, key) }
                );
                return new SftpClient(conn);
            }
            return new SftpClient(auth.Hostname, auth.Port, auth.Username, auth.Password);
        }

        private static string Run(SSHAuth auth, string cmdText)
        {
            using var ssh = CreateSsh(auth);
            ssh.Connect();
            var cmd = ssh.CreateCommand(cmdText);
            var output = cmd.Execute();
            var exit = cmd.ExitStatus;
            var err = cmd.Error;
            ssh.Disconnect();

            if (exit != 0 && string.IsNullOrEmpty(output))
                throw new Exception($"Command failed (exit {exit}): {err}");

            return output;
        }
    }
}
