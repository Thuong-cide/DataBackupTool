using DataBackupTool.Forms;
using DataBackupTool.Services;

namespace DataBackupTool;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        var scheduler = new SchedulerService();
        Application.Run(new MainForm(scheduler));
    }
}
