namespace DataBackupTool.Models
{
    public enum BackupMode
    {
        Backup,
        Sync
    }

    public class BackupDestination
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public List<string> SourcePaths { get; set; } = new();
        public string DestinationPath { get; set; } = "";
        public BackupMode Mode { get; set; } = BackupMode.Backup;
        public List<string> ExcludePatterns { get; set; } = new() { "*.tmp", "node_modules" };
        public bool IsScheduleEnabled { get; set; } = false;
        public string ScheduleTime { get; set; } = "22:00";
        public bool IsRealtimeEnabled { get; set; } = false;
    }
}
