using System.Threading;
using DataBackupTool.Models;

namespace DataBackupTool.Services
{
    public class RealtimeWatcherService
    {
        private readonly List<FileSystemWatcher> _watchers = new();
        private readonly Dictionary<string, System.Threading.Timer> _debounceTimers = new();
        private readonly Dictionary<string, SemaphoreSlim> _runningLocks = new();
        private readonly BackupService _backupService = new();
        private readonly SyncService _syncService = new();
        private const int DebounceMilliseconds = 5000;

        public event Action<string, BackupResult>? BackupCompleted;

        public void StartWatching(List<BackupDestination> destinations)
        {
            StopWatching();

            foreach (var destination in destinations.Where(d => d.IsRealtimeEnabled))
            {
                _runningLocks[destination.Id] = new SemaphoreSlim(1, 1);

                foreach (var sourcePath in destination.SourcePaths)
                {
                    if (!Directory.Exists(sourcePath))
                    {
                        continue;
                    }

                    var watcher = new FileSystemWatcher(sourcePath)
                    {
                        IncludeSubdirectories = true,
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName
                            | NotifyFilters.LastWrite | NotifyFilters.Size,
                        EnableRaisingEvents = true
                    };

                    watcher.Changed += (_, e) => OnFileChanged(destination, e.FullPath);
                    watcher.Created += (_, e) => OnFileChanged(destination, e.FullPath);
                    watcher.Deleted += (_, e) => OnFileChanged(destination, e.FullPath);
                    watcher.Renamed += (_, e) => OnFileChanged(destination, e.FullPath);

                    _watchers.Add(watcher);
                }
            }
        }

        private void OnFileChanged(BackupDestination destination, string? fullPath)
        {
            if (IsExcluded(fullPath, destination.ExcludePatterns))
            {
                return;
            }

            if (_debounceTimers.TryGetValue(destination.Id, out var existingTimer))
            {
                existingTimer.Change(DebounceMilliseconds, Timeout.Infinite);
            }
            else
            {
                var timer = new System.Threading.Timer(_ => RunBackupSafely(destination).GetAwaiter().GetResult(), null, DebounceMilliseconds, Timeout.Infinite);
                _debounceTimers[destination.Id] = timer;
            }
        }

        private async Task RunBackupSafely(BackupDestination destination)
        {
            if (!_runningLocks.TryGetValue(destination.Id, out var runningLock))
            {
                runningLock = new SemaphoreSlim(1, 1);
                _runningLocks[destination.Id] = runningLock;
            }

            if (!await runningLock.WaitAsync(0))
            {
                return;
            }

            try
            {
                var result = destination.Mode == BackupMode.Sync
                    ? _syncService.RunSync(destination)
                    : _backupService.RunBackup(destination);

                WriteLog($"[Realtime] [{destination.Name}] Copied: {result.FilesCopied}, Skipped: {result.FilesSkipped}, Errors: {result.Errors.Count}");
                BackupCompleted?.Invoke(destination.Name, result);
            }
            catch (Exception ex)
            {
                WriteLog($"[Realtime] [{destination.Name}] Lỗi: {ex.Message}");
            }
            finally
            {
                runningLock.Release();
            }
        }

        private bool IsExcluded(string? fullPath, List<string> patterns)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return false;
            }

            foreach (var pattern in patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern))
                {
                    continue;
                }

                var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") + "$";
                var fileName = Path.GetFileName(fullPath);
                var dirParts = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (System.Text.RegularExpressions.Regex.IsMatch(fileName, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    return true;
                if (dirParts.Any(p => System.Text.RegularExpressions.Regex.IsMatch(p, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                    return true;
            }

            return false;
        }

        private void WriteLog(string message)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
            File.AppendAllText(logFile, $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");
        }

        public void StopWatching()
        {
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _watchers.Clear();

            foreach (var timer in _debounceTimers.Values)
            {
                timer.Dispose();
            }
            _debounceTimers.Clear();
        }
    }
}
