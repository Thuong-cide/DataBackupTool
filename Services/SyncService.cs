using System.Text.Json;
using DataBackupTool.Models;

namespace DataBackupTool.Services
{
    public class SyncService
    {
        private const string ManifestFileName = ".backuptool_manifest.json";

        public BackupResult RunSync(BackupDestination destination)
        {
            var result = new BackupResult();

            if (!Directory.Exists(destination.DestinationPath))
            {
                result.Errors.Add($"Không truy cập được đích: {destination.DestinationPath}");
                return result;
            }

            foreach (var sourceRoot in destination.SourcePaths)
            {
                if (!Directory.Exists(sourceRoot))
                {
                    result.Errors.Add($"Không tìm thấy nguồn: {sourceRoot}");
                    continue;
                }

                var subFolderName = SanitizeSourceLabel(sourceRoot);
                var destRoot = Path.Combine(destination.DestinationPath, subFolderName);
                Directory.CreateDirectory(destRoot);

                SyncOneFolder(sourceRoot, destRoot, destination.ExcludePatterns, result);
            }

            return result;
        }

        private void SyncOneFolder(string sourceRoot, string destRoot, List<string> excludePatterns, BackupResult result)
        {
            var manifestPath = Path.Combine(destRoot, ManifestFileName);
            var manifest = LoadManifest(manifestPath);
            var newManifest = new Dictionary<string, long>();

            var sourceFiles = Directory.Exists(sourceRoot)
                ? Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories)
                    .Where(f => !IsExcluded(f, excludePatterns))
                    .ToDictionary(f => Path.GetRelativePath(sourceRoot, f), f => f)
                : new Dictionary<string, string>();

            var destFiles = Directory.EnumerateFiles(destRoot, "*", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f) != ManifestFileName && !IsExcluded(f, excludePatterns))
                .ToDictionary(f => Path.GetRelativePath(destRoot, f), f => f);

            var allRelativePaths = new HashSet<string>(sourceFiles.Keys);
            allRelativePaths.UnionWith(destFiles.Keys);
            allRelativePaths.UnionWith(manifest.Keys);

            foreach (var relativePath in allRelativePaths)
            {
                try
                {
                    bool inSource = sourceFiles.TryGetValue(relativePath, out var sourceFullPath);
                    bool inDest = destFiles.TryGetValue(relativePath, out var destFullPath);
                    bool inManifest = manifest.ContainsKey(relativePath);

                    if (inSource && inDest)
                    {
                        var sourceInfo = new FileInfo(sourceFullPath!);
                        var destInfo = new FileInfo(destFullPath!);

                        if (sourceInfo.LastWriteTimeUtc > destInfo.LastWriteTimeUtc)
                        {
                            File.Copy(sourceFullPath!, destFullPath!, overwrite: true);
                            result.FilesCopied++;
                        }
                        else if (destInfo.LastWriteTimeUtc > sourceInfo.LastWriteTimeUtc)
                        {
                            File.Copy(destFullPath!, sourceFullPath!, overwrite: true);
                            result.FilesCopied++;
                        }
                        else
                        {
                            result.FilesSkipped++;
                        }

                        newManifest[relativePath] = sourceInfo.LastWriteTimeUtc.Ticks;
                    }
                    else if (inSource && !inDest)
                    {
                        if (inManifest)
                        {
                            File.Delete(sourceFullPath!);
                            result.FilesCopied++;
                        }
                        else
                        {
                            var destPath = Path.Combine(destRoot, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                            File.Copy(sourceFullPath!, destPath, overwrite: true);
                            result.FilesCopied++;
                            newManifest[relativePath] = new FileInfo(sourceFullPath!).LastWriteTimeUtc.Ticks;
                        }
                    }
                    else if (!inSource && inDest)
                    {
                        if (inManifest)
                        {
                            File.Delete(destFullPath!);
                            result.FilesCopied++;
                        }
                        else
                        {
                            var sourcePath = Path.Combine(sourceRoot, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(sourcePath)!);
                            File.Copy(destFullPath!, sourcePath, overwrite: true);
                            result.FilesCopied++;
                            newManifest[relativePath] = new FileInfo(destFullPath!).LastWriteTimeUtc.Ticks;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"{relativePath}: {ex.Message}");
                }
            }

            SaveManifest(manifestPath, newManifest);
        }

        private Dictionary<string, long> LoadManifest(string manifestPath)
        {
            if (!File.Exists(manifestPath))
            {
                return new Dictionary<string, long>();
            }

            try
            {
                var json = File.ReadAllText(manifestPath);
                return JsonSerializer.Deserialize<Dictionary<string, long>>(json) ?? new();
            }
            catch
            {
                return new Dictionary<string, long>();
            }
        }

        private void SaveManifest(string manifestPath, Dictionary<string, long> manifest)
        {
            var json = JsonSerializer.Serialize(manifest);
            File.WriteAllText(manifestPath, json);
        }

        private string SanitizeSourceLabel(string sourceRoot)
        {
            var cleaned = sourceRoot.Replace(":", "")
                .Replace("\\", "_")
                .Replace("/", "_")
                .Trim('_');
            return string.IsNullOrWhiteSpace(cleaned) ? "Source" : cleaned;
        }

        private bool IsExcluded(string filePath, List<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern))
                {
                    continue;
                }

                var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") + "$";
                var fileName = Path.GetFileName(filePath);
                var dirParts = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (System.Text.RegularExpressions.Regex.IsMatch(fileName, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    return true;
                if (dirParts.Any(p => System.Text.RegularExpressions.Regex.IsMatch(p, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                    return true;
            }
            return false;
        }
    }
}
