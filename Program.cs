using DataBackupTool.Forms;
using DataBackupTool.Services;

namespace DataBackupTool;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        var configService = new ConfigService();
        var scheduler = new SchedulerService();
        var realtimeWatcher = new RealtimeWatcherService();
        realtimeWatcher.StartWatching(configService.GetAllDestinations());
        Application.Run(new MainForm(scheduler, realtimeWatcher));
    }
}
