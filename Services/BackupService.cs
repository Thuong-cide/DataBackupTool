using System.Text.RegularExpressions;
using DataBackupTool.Models;

namespace DataBackupTool.Services
{
    public class BackupResult
    {
        public int FilesCopied { get; set; }
        public int FilesSkipped { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class BackupService
    {
        public BackupResult RunBackup(BackupDestination destination)
        {
            var result = new BackupResult();

            if (!IsDestinationReachable(destination.DestinationPath))
            {
                result.Errors.Add($"Không truy cập được đích: {destination.DestinationPath} (kiểm tra kết nối mạng / quyền chia sẻ)");
                return result;
            }

            foreach (var sourceRoot in destination.SourcePaths)
            {
                if (!Directory.Exists(sourceRoot))
                {
                    result.Errors.Add($"Không tìm thấy nguồn: {sourceRoot}");
                    continue;
                }

                string subFolderName = SanitizeSourceLabel(sourceRoot);
                string destRootForThisSource = Path.Combine(destination.DestinationPath, subFolderName);
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
                        bool needCopy = sourceInfo.LastWriteTimeUtc > destInfo.LastWriteTimeUtc || sourceInfo.Length != destInfo.Length;

                        if (needCopy)
                        {
                            File.Copy(filePath, destPath, overwrite: true);
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

        private bool IsDestinationReachable(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return false;
                }

                if (Directory.Exists(path))
                {
                    return true;
                }

                Directory.CreateDirectory(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsExcluded(string filePath, List<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (string.IsNullOrWhiteSpace(pattern))
                {
                    continue;
                }

                var regexPattern = "^" + Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") + "$";
                var fileName = Path.GetFileName(filePath);
                var dirParts = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (Regex.IsMatch(fileName, regexPattern, RegexOptions.IgnoreCase))
                    return true;
                if (dirParts.Any(p => Regex.IsMatch(p, regexPattern, RegexOptions.IgnoreCase)))
                    return true;
            }
            return false;
        }
    }
}
