using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CRM_Alunos.Services;

public class UpdateService
{
    private const string GitHubApiUrl = "https://api.github.com/repos/mineblox99los/CRM-Install/releases/latest";
    private static readonly HttpClient _client = new();
    private static readonly string VersionFile = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "version.txt");

    public static string CurrentVersion { get; } = LoadCurrentVersion();

    private static string LoadCurrentVersion()
    {
        try
        {
            if (File.Exists(VersionFile))
                return File.ReadAllText(VersionFile).Trim();
        }
        catch { }
        return "1.0.0";
    }

    public static void SaveCurrentVersion(string version)
    {
        try { File.WriteAllText(VersionFile, version); } catch { }
    }

    public async Task<UpdateInfo?> CheckForUpdateAsync()
    {
        try
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("CRM-Install/1.0");
            var response = await _client.GetStringAsync(GitHubApiUrl);
            var release = JsonSerializer.Deserialize<GitHubRelease>(response);

            if (release == null || release.tag_name == null)
                return null;

            var latestVersion = release.tag_name.Replace("v", "").Split('-')[0];
            var latestParts = latestVersion.Split('.').Select(int.Parse).ToArray();
            var currentParts = CurrentVersion.Split('.').Select(int.Parse).ToArray();

            bool hasUpdate = false;
            for (int i = 0; i < Math.Max(latestParts.Length, currentParts.Length); i++)
            {
                int latest = i < latestParts.Length ? latestParts[i] : 0;
                int current = i < currentParts.Length ? currentParts[i] : 0;
                if (latest > current) { hasUpdate = true; break; }
                if (latest < current) break;
            }

            if (!hasUpdate) return null;

            var asset = release.assets?.FirstOrDefault(a => a.name?.EndsWith(".zip") == true);

            return new UpdateInfo
            {
                Version = latestVersion,
                TagName = release.tag_name,
                Name = release.name ?? "",
                Body = release.body ?? "",
                DownloadUrl = asset?.browser_download_url ?? "",
                FileName = asset?.name ?? "CRM-Alunos.zip"
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DownloadAndInstallAsync(UpdateInfo update, Action<string>? progress = null)
    {
        try
        {
            progress?.Invoke("Baixando atualizacao...");
            var tempPath = Path.Combine(Path.GetTempPath(), update.FileName);
            var zipBytes = await _client.GetByteArrayAsync(update.DownloadUrl);
            await File.WriteAllBytesAsync(tempPath, zipBytes);

            progress?.Invoke("Extraindo arquivos...");
            var extractPath = Path.Combine(Path.GetTempPath(), "CRM-Update");
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);
            ZipFile.ExtractToDirectory(tempPath, extractPath);

            progress?.Invoke("Instalando atualizacao...");
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var exePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
            var batPath = Path.Combine(Path.GetTempPath(), "CRM-Update.bat");

            var batContent = $@"
@echo off
timeout /t 2 /nobreak > nul
taskkill /f /im CRM-Alunos.exe 2>nul
timeout /t 1 /nobreak > nul
xcopy /s /y /e ""{extractPath}\*"" ""{appDir}"" 
del ""{tempPath}""
rmdir /s /q ""{extractPath}""
start """" ""{exePath}""
del ""%~f0""
";
            await File.WriteAllTextAsync(batPath, batContent);

            progress?.Invoke("Reiniciando aplicacao...");
            SaveCurrentVersion(update.Version);
            Process.Start(new ProcessStartInfo
            {
                FileName = batPath,
                UseShellExecute = true,
                CreateNoWindow = true
            });

            Application.Current.Shutdown();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class UpdateInfo
{
    public string Version { get; set; } = "";
    public string TagName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Body { get; set; } = "";
    public string DownloadUrl { get; set; } = "";
    public string FileName { get; set; } = "";
}

public class GitHubRelease
{
    public string? tag_name { get; set; }
    public string? name { get; set; }
    public string? body { get; set; }
    public GitHubAsset[]? assets { get; set; }
}

public class GitHubAsset
{
    public string? name { get; set; }
    public string? browser_download_url { get; set; }
}
