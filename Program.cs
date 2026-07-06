using DataBackupTool.Forms;
using DataBackupTool.Services;

namespace DataBackupTool;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        var configService = new ConfigService();
        var scheduler = new SchedulerService();
        var realtimeWatcher = new RealtimeWatcherService();
        realtimeWatcher.StartWatching(configService.GetAllDestinations());

        var mainForm = new MainForm(scheduler, realtimeWatcher);

        if (args.Contains("--minimized"))
        {
            mainForm.Load += (_, _) => mainForm.StartHidden();
        }

        Application.Run(mainForm);
    }
}
