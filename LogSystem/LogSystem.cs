using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogSystem
{
    public class LogSystem : ILogSystem
    {
        // Paths & names
        private readonly string _configPathName;     // e.g. <app>/config/LogConfig.xml
        private readonly string _logFolderName;      // e.g. "MyAppLogs"
        private readonly string _logFileNamePrefix;  // e.g. "EBSApp"

        // State
        private LogConfig _config = new();
        private string _currentLogFile = string.Empty;
        private int _fileIndex = 1;
        private string _currentDate = DateTime.Now.ToString("yyyyMMdd");
        private readonly object _lock = new();

        /// <summary>
        /// Create a logger with a given log folder and log file prefix.
        /// Config is auto-read from <app>/config/LogConfig.xml (created if missing).
        /// </summary>
        public LogSystem(string logFolderName, string logFileNamePrefix)
        {
            _logFolderName = string.IsNullOrWhiteSpace(logFolderName) ? "Logs" : logFolderName;
            _logFileNamePrefix = string.IsNullOrWhiteSpace(logFileNamePrefix) ? "SystemLog" : logFileNamePrefix;

            // Config under ./config/LogConfig.xml
            var appBase = AppContext.BaseDirectory;
            var cfgDir = Path.Combine(appBase, "config");
            Directory.CreateDirectory(cfgDir);
            _configPathName = Path.Combine(cfgDir, "LogConfig.xml");

            EnsureConfigExists();   // create default XML if missing
            LoadConfig();           // read XML (fallback to defaults if partial)
            Directory.CreateDirectory(_logFolderName);

            CleanupOldLogs();
            UpdateLogFile();
        }

        private void EnsureConfigExists()
        {
            if (File.Exists(_configPathName)) return;

            // Create default config XML
            var serializer = new XmlSerializer(typeof(LogConfig));
            using var sw = new StreamWriter(_configPathName);
            serializer.Serialize(sw, new LogConfig()); // writes defaults: RetentionDays=7, MaxFileSizeMB=10
        }

        private void LoadConfig()
        {
            try
            {
                if (!File.Exists(_configPathName)) return;

                var serializer = new XmlSerializer(typeof(LogConfig));
                using var reader = new StreamReader(_configPathName);
                var loaded = (LogConfig?)serializer.Deserialize(reader);
                if (loaded != null)
                {
                    // Merge (defaults already set in _config)
                    _config.RetentionDays = loaded.RetentionDays;
                    _config.MaxFileSizeMB = loaded.MaxFileSizeMB;
                }
            }
            catch
            {
                // Swallow errors; keep defaults
            }
        }

        private void UpdateLogFile()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            _currentDate = datePart;

            while (true)
            {
                string fileName = $"{_logFileNamePrefix}_{datePart}_{_fileIndex}.txt";
                _currentLogFile = Path.Combine(_logFolderName, fileName);

                if (!File.Exists(_currentLogFile)) break;

                long sizeMB = new FileInfo(_currentLogFile).Length / (1024 * 1024);
                if (sizeMB < _config.MaxFileSizeMB) break;

                _fileIndex++;
            }
        }

        private void RotateIfNeeded()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            if (today != _currentDate)
            {
                _fileIndex = 1;
                UpdateLogFile();
                return;
            }

            if (!File.Exists(_currentLogFile)) return;

            long sizeMB = new FileInfo(_currentLogFile).Length / (1024 * 1024);
            if (sizeMB >= _config.MaxFileSizeMB)
            {
                _fileIndex++;
                UpdateLogFile();
            }
        }

        private void CleanupOldLogs()
        {
            if (_config.RetentionDays <= 0) return;

            var di = new DirectoryInfo(_logFolderName);
            if (!di.Exists) return;

            // Only delete files that match this logger's prefix
            foreach (var f in di.GetFiles($"{_logFileNamePrefix}_*.txt", SearchOption.TopDirectoryOnly))
            {
                if (f.CreationTime < DateTime.Now.AddDays(-_config.RetentionDays))
                {
                    try { f.Delete(); } catch { /* ignore */ }
                }
            }
        }

        public void Write(string message, string level = "INFO")
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            lock (_lock)
            {
                RotateIfNeeded();
                File.AppendAllText(_currentLogFile, entry + Environment.NewLine);
            }
        }

        public void Info(string message) => Write(message, "INFO");
        public void Warning(string message) => Write(message, "WARNING");
        public void Error(string message) => Write(message, "ERROR");
    }
}
