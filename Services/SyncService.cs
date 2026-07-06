using DataBackupTool.Models;

namespace DataBackupTool.Services
{
    public class SyncService
    {
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
                var destRootForThisSource = Path.Combine(destination.DestinationPath, subFolderName);
                Directory.CreateDirectory(destRootForThisSource);

                foreach (var filePath in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (IsExcluded(filePath, destination.ExcludePatterns))
                        {
                            result.FilesSkipped++;
                            continue;
                        }

                        var relativePath = Path.GetRelativePath(sourceRoot, filePath);
                        var destPath = Path.Combine(destRootForThisSource, relativePath);
                        var destDir = Path.GetDirectoryName(destPath)!;
                        Directory.CreateDirectory(destDir);

                        if (!File.Exists(destPath))
                        {
                            File.Copy(filePath, destPath, overwrite: true);
                            result.FilesCopied++;
                            continue;
                        }

                        var sourceInfo = new FileInfo(filePath);
                        var destInfo = new FileInfo(destPath);
                        var sourceNewer = sourceInfo.LastWriteTimeUtc > destInfo.LastWriteTimeUtc;
                        var destNewer = destInfo.LastWriteTimeUtc > sourceInfo.LastWriteTimeUtc;

                        if (sourceNewer)
                        {
                            File.Copy(filePath, destPath, overwrite: true);
                            result.FilesCopied++;
                        }
                        else if (destNewer)
                        {
                            File.Copy(destPath, filePath, overwrite: true);
                            result.FilesCopied++;
                        }
                        else
                        {
                            result.FilesSkipped++;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"{filePath}: {ex.Message}");
                    }
                }
            }

            return result;
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
