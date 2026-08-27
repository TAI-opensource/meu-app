namespace CRM_Alunos.Web.Services;

public class ErrorInfo
{
    public string Type { get; set; } = "";
    public string Message { get; set; } = "";
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string>? AdditionalData { get; set; }
}

public interface IDebugService
{
    void CaptureError(Exception ex, string? context = null);
    void CaptureError(string type, string message, string? stackTrace = null);
    ErrorInfo? GetCurrentError();
    List<ErrorInfo> GetAllErrors();
    void ClearError();
    string GetVersion();
}

public class DebugService : IDebugService
{
    private readonly List<ErrorInfo> _errors = new();
    private ErrorInfo? _currentError;
    private const string VersionFile = "version.txt";
    private const string DefaultVersion = "1.0.0";

    public void CaptureError(Exception ex, string? context = null)
    {
        var error = new ErrorInfo
        {
            Type = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Timestamp = DateTime.UtcNow,
            AdditionalData = new Dictionary<string, string>()
        };

        if (!string.IsNullOrEmpty(context))
            error.AdditionalData["Contexto"] = context;

        if (ex.InnerException != null)
        {
            error.AdditionalData["InnerException"] = $"{ex.InnerException.GetType().Name}: {ex.InnerException.Message}";
        }

        _errors.Add(error);
        _currentError = error;
    }

    public void CaptureError(string type, string message, string? stackTrace = null)
    {
        var error = new ErrorInfo
        {
            Type = type,
            Message = message,
            StackTrace = stackTrace,
            Timestamp = DateTime.UtcNow
        };

        _errors.Add(error);
        _currentError = error;
    }

    public ErrorInfo? GetCurrentError() => _currentError;

    public List<ErrorInfo> GetAllErrors() => new(_errors);

    public void ClearError()
    {
        _currentError = null;
    }

    public string GetVersion()
    {
        try
        {
            var versionPath = Path.Combine(AppContext.BaseDirectory, VersionFile);
            if (File.Exists(versionPath))
                return File.ReadAllText(versionPath).Trim();
        }
        catch { }
        return DefaultVersion;
    }
}
