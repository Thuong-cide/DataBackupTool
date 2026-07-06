using System.Text.Json;
using DataBackupTool.Models;

namespace DataBackupTool.Services
{
    public class ConfigService
    {
        private readonly string _configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        public List<BackupDestination> GetAllDestinations()
        {
            if (!File.Exists(_configPath))
            {
                return new List<BackupDestination>();
            }

            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<List<BackupDestination>>(json) ?? new List<BackupDestination>();
        }

        public void SaveDestinations(List<BackupDestination> destinations)
        {
            var json = JsonSerializer.Serialize(destinations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }

        public void AddDestination(BackupDestination destination)
        {
            var list = GetAllDestinations();
            list.Add(destination);
            SaveDestinations(list);
        }

        public void UpdateDestination(BackupDestination updated)
        {
            var list = GetAllDestinations();
            var index = list.FindIndex(d => d.Id == updated.Id);
            if (index >= 0)
            {
                list[index] = updated;
            }
            else
            {
                list.Add(updated);
            }

            SaveDestinations(list);
        }

        public void RemoveDestination(string id)
        {
            var list = GetAllDestinations();
            list.RemoveAll(d => d.Id == id);
            SaveDestinations(list);
        }
    }
}
