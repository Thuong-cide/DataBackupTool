using DataBackupTool.Models;

namespace DataBackupTool.Services
{
    public class SchedulerService : IDisposable
    {
        private readonly System.Threading.Timer _timer;
        private readonly ConfigService _configService = new();
        private readonly BackupService _backupService = new();
        private readonly SyncService _syncService = new();

        public SchedulerService()
        {
            _timer = new System.Threading.Timer(CheckSchedule, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        public void RunNow(BackupDestination destination)
        {
            try
            {
                var result = destination.Mode == BackupMode.Sync
                    ? _syncService.RunSync(destination)
                    : _backupService.RunBackup(destination);

                WriteLog($"[{destination.Name}] Copied: {result.FilesCopied}, Skipped: {result.FilesSkipped}, Errors: {result.Errors.Count}");
            }
            catch (Exception ex)
            {
                WriteLog($"[{destination.Name}] Lỗi: {ex.Message}");
            }
        }

        private void CheckSchedule(object? state)
        {
            try
            {
                var now = DateTime.Now.ToString("HH:mm");
                var destinations = _configService.GetAllDestinations();

                foreach (var destination in destinations.Where(d => d.IsScheduleEnabled && d.ScheduleTime == now))
                {
                    RunNow(destination);
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Lỗi lịch trình: {ex.Message}");
            }
        }

        private void WriteLog(string message)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
            File.AppendAllText(logFile, $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
